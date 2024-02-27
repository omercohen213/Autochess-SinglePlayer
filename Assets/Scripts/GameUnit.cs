using HeroEditor.Common.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

public class GameUnit : Unit, IDamageable
{
    private Animator _animator;

    private Player _owner;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _hp;
    [SerializeField] private int _maxMp;
    [SerializeField] private int _mp;
    [SerializeField] private int _baseAttackDamage;
    [SerializeField] private int _attackDamage;
    [SerializeField] private float _baseAttackSpeed;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private int _range;
    [SerializeField] private float _rangeSpeed;
    [SerializeField] private Weapon _weapon;

    private bool _isOnBoard;
    private Hex _currentHex; // Current hex spot if unit is on board. Null otherwise
    private BenchSlot _currentBenchSlot; // Bench spot if unit is on bench. Null otherwise.
    private Dictionary<Trait, int> _traitStages = new(); // Each trait the unit has and its current stage

    private int _starLevel;
    public readonly int MAX_STAR_LEVEL = 3;

    public Player Owner { get => _owner; set => _owner = value; }
    public int MaxHp { get => _maxHp; private set => _maxHp = value; }
    public int MaxMp { get => _maxMp; private set => _maxMp = value; }
    public int BaseAttackDamage { get => _baseAttackDamage; private set => _baseAttackDamage = value; }
    public int Range { get => _range; set => _range = value; }
    public bool IsOnBoard { get => _isOnBoard; set => _isOnBoard = value; }
    public Hex CurrentHex { get => _currentHex; set => _currentHex = value; }
    public BenchSlot CurrentBenchSlot { get => _currentBenchSlot; set => _currentBenchSlot = value; }
    public Dictionary<Trait, int> TraitStages { get => _traitStages; set => _traitStages = value; }
    public int AttackDamage { get => _attackDamage; set => _attackDamage = value; }
    public int StarLevel { get => _starLevel; set => _starLevel = value; }
    public int Hp { get => _hp; set => _hp = value; }
    public int Mp { get => _mp; set => _mp = value; }
    public float BaseAttackSpeed { get => _baseAttackSpeed; set => _baseAttackSpeed = value; }
    public float AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }
    public Weapon Weapon { get => _weapon; set => _weapon = value; }

    private void Awake()
    {
        if (_owner == LocalPlayer.Instance)
        {
            Transform animationTransform = transform.Find("Animation");
            if (animationTransform != null)
            {
                _animator = animationTransform.GetComponent<Animator>();
            }
            else
            {
                Debug.LogWarning("Missing animation transform on game unit " + _unitName);
            }
        }
        // Monster animation
        else
        {
            _animator = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
        GameUnitAttack.OnAttack += OnAttack;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
        GameUnitAttack.OnAttack -= OnAttack;
    }

    public void Initialize(Player owner, UnitData unitData, int starLevel)
    {
        _owner = owner;
        SetUnitData(unitData, starLevel);
        ShowStars(starLevel);

        _hp = _maxHp;
        _mp = 0;
        _attackDamage = _baseAttackDamage;
        _attackSpeed = _baseAttackSpeed;

        // Initialize all traits with stage 0
        foreach (var trait in Traits)
        {
            _traitStages.Add(trait, 0);
        }
    }

    public void SetUnitData(UnitData unitData, int starLevel)
    {
        base.SetUnitData(unitData);
        float multiplier = 1;
        switch (starLevel)
        {
            case 1:
                multiplier = 1;
                break;
            case 2:
                multiplier = 1.5f;
                break;
            case 3:
                multiplier = 3;
                break;
        }
        _weapon = _unitData.Weapon;
        _maxHp = (int)(_unitData.MaxHp * multiplier);
        _maxMp = _unitData.MaxMp;
        _baseAttackDamage = (int)(_unitData.BaseAttackDamage * multiplier);
        _baseAttackSpeed = (int)(_unitData.BaseAttackSpeed * multiplier);
        _range = _unitData.Range;
        _starLevel = starLevel;

    }

    private void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                break;
            case GamePhase.Battle:
                break;
            case GamePhase.BattleResult:
                break;
        }
    }

    public void HandleDragStarted()
    {
        Shop.Instance.ActivateUnitSellField(_cost);
    }

    // Handles a behavior when this unit is stopped being dragged at final position
    public void HandleDragStopped(GameObject objDraggedOn)
    {
        string objTag = objDraggedOn != null ? objDraggedOn.tag : null;

        // The unit was not dragged on a game object - return unit to its current bench or hex
        if (objTag == null)
        {
            ReturnToPlace();
        }
        else
        {
            switch (objTag)
            {
                // Sell Unit
                case "SellField":
                    Shop.Instance.SellUnit(this);
                    Destroy(gameObject);
                    break;

                // Place on board
                case "Hex":
                    Hex hex = objDraggedOn.GetComponent<Hex>();
                    CheckHexPlacement(hex);
                    break;

                // Place on another bench slot
                case "BenchSlot":
                    BenchSlot benchSlotDraggedOn = objDraggedOn.GetComponent<BenchSlot>();
                    _owner.Bench.PlaceOnBenchSlot(this, benchSlotDraggedOn);
                    break;

                default:
                    ReturnToPlace();
                    break;
            }
        }
        Shop.Instance.DisableUnitSellField();
    }

    // Place unit on board only if allowed
    private void CheckHexPlacement(Hex hex)
    {
        // Allow only if board limit is not reached
        if (Owner.IsBoardLimitReached() && !IsOnBoard)
        {
            ReturnToPlace();
        }

        // Allow only on player's side of the board
        else if (hex.X < 4)
        {
            Board.Instance.PlaceUnitOnBoard(this, hex);
        }
        else
        {
            ReturnToPlace();
            Debug.Log("Can't place unit on opponent's board");
        }

    }

    // Return unit to its current bench or hex
    private void ReturnToPlace()
    {
        if (IsOnBoard)
        {
            Board.Instance.PlaceUnitOnBoard(this, CurrentHex);
        }
        else
        {
            _owner.Bench.PlaceOnBenchSlot(this, _currentBenchSlot);
        }
    }

    // Create star objects and set current star level
    public void ShowStars(int starLevel)
    {
        Transform starsParent = transform.Find("Stars");
        if (starsParent != null)
        {
            for (int i = 0; i < starLevel; i++)
            {
                if (starsParent.childCount >= starLevel)
                {

                    starsParent.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("Missing star objets on unit " + _unitName);
                }
            }
        }
        else
        {
            Debug.LogWarning("Missing star parent on gameUnit " + _unitName);
        }
    }

    public bool Equals(GameUnit other)
    {
        return UnitName == other.UnitName;
    }

    public void PlaceOnBenchSlot(BenchSlot benchSlot)
    {
        benchSlot.UnitOnSlot = this;
        _currentBenchSlot = benchSlot;
        benchSlot.IsTaken = true;
        transform.position = _currentBenchSlot.transform.position;
        transform.SetParent(benchSlot.transform);
    }

    public void RemoveFromBench()
    {
        if (_currentBenchSlot != null)
        {
            _currentBenchSlot.UnitOnSlot = null;
            _currentBenchSlot.IsTaken = false;
            _currentBenchSlot = null;
        }
    }

    // Places a unit on given hex
    public void PlaceOnHex(Hex hex)
    {
        if (hex == null)
        {
            Debug.LogWarning("hex is null");
            return;
        }
        if (_currentHex != null && _currentHex.UnitOnHex == this)
        {
            _currentHex.IsTaken = false;
            _currentHex.UnitOnHex = null;
        }
        _currentHex = hex;
        hex.IsTaken = true;
        hex.UnitOnHex = this;
        _isOnBoard = true;
        transform.SetParent(hex.transform);
        transform.position = hex.transform.position;
    }

    // Changing unit's hex without changing its' position
    public void ChangeHex(Hex hex)
    {
        if (hex == null)
        {
            Debug.LogWarning("hex is null");
            return;
        }
        if (_currentHex != null)
        {
            _currentHex.IsTaken = false;
        }
        _currentHex.UnitOnHex = null;
        _currentHex = hex;
        hex.IsTaken = true;
        hex.UnitOnHex = this;
        _isOnBoard = true;
    }

    public void RemoveFromBoard()
    {
        if (_currentHex != null)
        {
            _currentHex.IsTaken = false;
            _currentHex.UnitOnHex = null;
            _currentHex = null;
        }
        _isOnBoard = false;
        _owner.BoardUnits.Remove(this);
    }

    public List<GameUnit> GetEnemyUnits()
    {
        if (_owner == LocalPlayer.Instance)
        {
            return Opponent.Instance.BoardUnits;
        }
        else
        {
            return LocalPlayer.Instance.BoardUnits;
        }
    }

    public void OnAttack(GameUnit attacker, GameUnit target, int damage)
    {
        if (attacker == this)
        {
            AnimateAttack();

            _mp += 10;
            if (_mp == _maxMp)
            {
                UseAbility(target);
                _mp = 0;
            }

            target.OnDamageTaken(damage);
            if (!target.IsDead())
            {
                UIManager.UpdateUnitMPBar(this);
            }
        }
    }

    private void AnimateAttack()
    {
        if (_animator != null)
        {
            switch (_weapon)
            {
                case Weapon.MeeleOneHanded:
                    _animator.SetTrigger("Slash1H");
                    break;
                case Weapon.MeeleTwoHanded:
                    _animator.SetTrigger("Slash2H");
                    break;
                case Weapon.Staff:
                    // todo: change to jab if close range
                    _animator.SetTrigger("Cast");
                    break;
                case Weapon.Bow:
                    _animator.SetTrigger("SimpleBowShot");
                    break;
                case Weapon.Gun:
                    break;
                case Weapon.NoWeapon:
                    _animator.SetTrigger("Attack");
                    break;
            }
        }
    }

    public IEnumerator BowShot()
    {
        _animator.SetInteger("Charge", 1); // 0 = ready, 1 = charging, 2 = release, 3 = cancel.

        yield return new WaitForSeconds(1);

        _animator.SetInteger("Charge", 2);

        yield return new WaitForSeconds(1);

        _animator.SetInteger("Charge", 0);
    }

    public void OnDamageTaken(int damage)
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Hit");
        }
        _hp -= damage;
        if (IsDead())
        {
            _hp = 0;
            Die();
        }
        else
        {
            UIManager.UpdateUnitHPBar(this);
        }
    }

    public bool IsDead()
    {
        return _hp <= 0;
    }

    private void UseAbility(GameUnit target)
    {
        Debug.Log("ability Used " + _unitName);
    }

    public void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {

        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    public void ShowBars()
    {
        Transform bars = transform.Find("Bars");
        if (bars != null)
        {
            bars.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Missing bars object");
        }
        UIManager.UpdateUnitHPBar(this);
        UIManager.UpdateUnitMPBar(this);
    }
}