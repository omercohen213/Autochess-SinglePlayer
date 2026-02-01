using HeroEditor.Common.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using static UnityEngine.UI.CanvasScaler;
using TMPro.EditorUtilities;
using TMPro;
using Assets.HeroEditor.Common.Scripts.ExampleScripts;
using UnityEditor;
using Random = UnityEngine.Random;



// currently every unit must attack and move
[RequireComponent(typeof(GameUnitMovement))]
[RequireComponent(typeof(GameUnitAttack))]
public class GameUnit : Unit, IDamageable
{
    private Animator _animator;

    [SerializeField] private Player _owner;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _hp;
    [SerializeField] private int _maxMp;
    [SerializeField] private int _mp;
    [SerializeField] private int _baseAttackDamage;
    [SerializeField] private int _attackDamage;
    [SerializeField] private int _baseAbilityPower;
    [SerializeField] private int _abilityPower;
    [SerializeField] private int _baseArmor;
    [SerializeField] private int _armor;
    [SerializeField] private int _magicResist;
    [SerializeField] private int _baseMagicResist;
    [SerializeField] private float _baseAttackSpeed;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private int _range;
    [SerializeField] private float _critChance;
    [SerializeField] private float _critDamage;
    [SerializeField] private float _rangeSpeed;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private GameObject _weaponProjectilePrefab;

    [SerializeField] private Ability _ability;

    private bool _isOnBoard;
    private Hex _currentHex; // Current hex spot if unit is on board. Null otherwise
    private BenchSlot _currentBenchSlot; // Bench spot if unit is on bench. Null otherwise.
    private Dictionary<Trait, int> _traitStages = new(); // Each trait the unit has and its current stage   
    private RoundManager _roundManager;
    private GameManager _gameManager;

    public delegate void DeathEventHandler(GameUnit gameUnit);
    public static event DeathEventHandler OnDeath;

    private int _starLevel;
    public static readonly int MAX_STAR_LEVEL = 3;
    public readonly float DEATH_FADE_DURATION = 2f;

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
    public float CritChance { get => _critChance; set => _critChance = value; }
    public float CritDamage { get => _critDamage; set => _critDamage = value; }
    public int AbilityPower { get => _abilityPower; set => _abilityPower = value; }
    public int BaseAbilityPower { get => _baseAbilityPower; set => _baseAbilityPower = value; }
    public int Armor { get => _armor; set => _armor = value; }
    public int BaseArmor { get => _baseArmor; set => _baseArmor = value; }
    public int MagicResist { get => _magicResist; set => _magicResist = value; }
    public int BaseMagicResist { get => _baseMagicResist; set => _baseMagicResist = value; }
    public UnitState CurrentState { get => _currentState; set => _currentState = value; }
    public GameUnit CurrentTarget { get => _currentTarget; set => _currentTarget = value; }
    private void Awake()
    {
        _attack = GetComponent<GameUnitAttack>();
        _movement = GetComponent<GameUnitMovement>();
    }

    private void OnEnable()
    {
        _roundManager = RoundManager.Instance;
        _gameManager = GameManager.Instance;
        if (_roundManager != null)
        {
            _roundManager.OnPhaseChanged += OnPhaseChanged;
        }
        if (_gameManager != null)
        {
            GameUnitAttack.OnAttack += OnAttack;
        }
    }
    private void OnDisable()
    {
        if (_roundManager != null)
        {
            _roundManager.OnPhaseChanged -= OnPhaseChanged;
        }
        if (_gameManager != null)
        {
            GameUnitAttack.OnAttack -= OnAttack;
        }
    }
    /*    private void OnDisable()
        {
            //GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
            //GameUnitAttack.OnAttack -= OnAttack;
        }*/

    public void Initialize(Player owner, UnitData unitData, int starLevel)
    {
        _owner = owner;
        SetUnitData(unitData, starLevel);
        ShowStars(starLevel);
        _currentState = UnitState.Idle;

        // Starting stats
        _hp = _maxHp;
        _mp = 0;
        _attackDamage = _baseAttackDamage;
        _abilityPower = _baseAbilityPower;
        _armor = _baseArmor;
        _magicResist = _baseMagicResist;
        _attackSpeed = _baseAttackSpeed;
        _critChance = 0.2f; // base should be 0f
        _critDamage = 2f; // base should be 1f

        // Initialize all traits with stage 0
        foreach (var trait in Traits)
        {
            _traitStages.Add(trait, 0);
        }

        if (_owner == LocalPlayer.Instance)
        {
            Transform animationTransform = transform.Find("Character");
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
        _weaponProjectilePrefab = _unitData.WeaponProjectile;
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
            case GamePhase.RoundStart:
                break;
            case GamePhase.RoundOver:
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
            Board.PlaceUnitOnBoard(this, hex);
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
            Board.PlaceUnitOnBoard(this, CurrentHex);
        }
        else
        {
            _owner.Bench.PlaceOnBenchSlot(this, _currentBenchSlot);
        }
    }

    // Create star objects and set current star level
    public void ShowStars(int starLevel)
    {
        Transform[] childTransforms = transform.GetComponentsInChildren<Transform>(true);
        Transform starsParent = null;
        foreach (Transform child in childTransforms)
        {
            if (child.name == "Stars")
            {
                starsParent = child.gameObject.transform;
                starsParent.gameObject.SetActive(true);
            }
        }

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
            bool isCritical = IsCriticalAttack();
            if (isCritical)
            {
                damage = Mathf.RoundToInt(damage * _critDamage);
            }

            if (_mp == _maxMp)
            {
                if (_ability != null)
                {
                    _ability.CastAbility(this, target);
                }
                _mp = 0;
            }
            else
            {
                _mp += 10;
            }
            UpdateUnitMPBar();

            // Meele - no projectile
            if ((_weapon == Weapon.MeeleOneHanded) || (_weapon == Weapon.MeeleTwoHanded) || (_weapon == Weapon.NoWeapon))
            {
                target.OnDamageTaken(damage, false, isCritical);
            }
            else
            {
                ShootProjectile(_weaponProjectilePrefab, target, damage, false, isCritical);
            }
            AnimateAttack();
        }
    }

    private bool IsCriticalAttack()
    {
        return Random.value < _critChance;
    }

    private void AnimateAttack()
    {
        if (_animator != null)
        {
            _animator.SetFloat("Speed", _attackSpeed);
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
                    // BowShotAnimation();
                    break;
                case Weapon.Gun:
                    break;
                case Weapon.NoWeapon:
                    _animator.SetTrigger("Attack");
                    break;
            }
        }
    }

    public void ShootProjectile(GameObject projectilePrefab, GameUnit target, int damage, bool isMagic, bool isCritical)
    {
        if (projectilePrefab != null)
        {
            Vector3 startingPosition = transform.position + new Vector3(0, 0.5f);
            GameObject projectileGo = Instantiate(projectilePrefab, startingPosition, Quaternion.identity, transform);
            Projectile projectile = projectileGo.GetComponent<Projectile>();
            projectile.MoveProjectile(target, damage, isMagic, isCritical);
        }
        else
        {
            Debug.LogWarning("Missing projectile prefab");
        }
    }

    public void OnDamageTaken(int damage, bool isMagic, bool isCritical)
    {
        int damageTaken;
        if (isMagic)
        {
            damageTaken = damage - _armor;
        }
        else
        {
            damageTaken = damage - _magicResist;
        }

        TextType textType = TextType.Normal;
        if (isMagic)
        {
            textType = TextType.Magic;
        }
        else if (isCritical)
        {
            textType = TextType.Critical;
        }

        DynamicTextManager.CreateFloatingText(transform.position, damageTaken.ToString(), textType);
        _hp -= damageTaken;
        if (IsDead())
        {
            _hp = 0;
            Die();
        }
        else
        {
            UpdateUnitHPBar();
        }
    }

    public bool IsDead()
    {
        return _hp <= 0;
    }

    public void Die()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Death");
        }
        OnDeath?.Invoke(this);
        _currentState = UnitState.Dead;
        RemoveFromBoard();
        if (_owner == Opponent.Instance)
        {
            ItemDropManager.Instance.CreateItemOrb(transform.position);
        }
        StartCoroutine(DieCoroutine());
    }

    // Fade the character over time and then destroy it
    private IEnumerator DieCoroutine()
    {
        float initialAlpha = 1f;
        float elapsedTime = 0f;

        HideBars();
        HideStars();

        // Loop until the fade duration is reached
        while (elapsedTime < DEATH_FADE_DURATION)
        {
            // Calculate the current alpha value based on the elapsed time
            float currentAlpha = Mathf.Lerp(initialAlpha, 0f, elapsedTime / DEATH_FADE_DURATION);
            SetAlphaColor(currentAlpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlphaColor(0);
        Destroy(gameObject);

    }

    // Set alpha colors of all body parts of the character
    private void SetAlphaColor(float alphaColor)
    {
        // fix for monsters
        Transform characterTransform;
        if (_owner == LocalPlayer.Instance)
        {
            characterTransform = transform.Find("Character");

        }
        else
        {
            characterTransform = transform.Find("Body");

        }
        SpriteRenderer[] bodyParts = characterTransform.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer bodyPart in bodyParts)
        {
            if (bodyPart != null)
            {
                Color color = bodyPart.color;
                color.a = alphaColor;
                bodyPart.color = color;
            }
        }
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
        UpdateUnitHPBar();
        UpdateUnitMPBar();
    }

    private void HideBars()
    {
        Transform bars = transform.Find("Bars");
        if (bars != null)
        {
            bars.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Missing bars object");
        }
    }

    private void HideStars()
    {
        Transform starsParent = transform.Find("Stars");
        if (starsParent != null)
        {
            starsParent.gameObject.SetActive(false);
        }
    }

    public void OnHealRecieved(int healAmount)
    {
        DynamicTextManager.CreateFloatingText(transform.position, healAmount.ToString(), TextType.Heal);
        if (_hp + healAmount > _maxHp)
        {
            _hp = _maxHp;
        }
        else
        {
            _hp += healAmount;
        }
        UpdateUnitHPBar();
    }

    public void UpdateUnitHPBar()
    {
        if (transform.Find("Bars").Find("HpBar").TryGetComponent<Slider>(out var fill))
        {
            int maxHp = _maxHp;
            int currentHp = _hp;
            float fillAmount = (float)currentHp / maxHp;
            fill.value = fillAmount;
        }
        else
        {
            Debug.LogWarning("Missing HP bar objects");
        }
    }

    public void UpdateUnitMPBar()
    {
        if (transform.Find("Bars").Find("MpBar").TryGetComponent<Slider>(out var fill))
        {
            int maxMp = _maxMp;
            int currentMp = _mp;
            float fillAmount = (float)currentMp / +maxMp;
            fill.value = fillAmount;
        }
        else
        {
            Debug.LogWarning("Missing MP bar objects");
        }
    }

    public void AnimateMovement()
    {
        if (_animator != null)
        {
            _animator.SetBool("Walk", true);
        }
    }

    public void StopAnimateMovement()
    {
        if (_animator != null)
        {
            _animator.SetBool("Walk", false);
        }
    }

    public void ChangeState(UnitState newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case UnitState.Attacking:
                _attack.Attack(_currentTarget);
                break;

            case UnitState.Moving:
             //   _movement.BeginMoveState();
                break;

            case UnitState.Idle:
                //_movement.StopAllMovement();
                break;

            case UnitState.Dead:
               // _movement.StopAllMovement();
             //   _attack.StopAllAttacks();
                break;
        }


    }
}