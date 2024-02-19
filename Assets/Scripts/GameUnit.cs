using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

[RequireComponent(typeof(Animator))]
public class GameUnit : Unit
{
    [SerializeField] private Player _owner;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxMp;
    [SerializeField] private int _baseAttackDamage;
    [SerializeField] private int _range;
    [SerializeField] private GameObject _starPrefab;
    private Animator _animator;
    private GamePhase _currentGamePhase;

    public bool _isOnBoard;
    private Hex _currentHex; // Current hex spot if unit is on board. Null otherwise
    private BenchSlot _currentBenchSlot; // Bench spot if unit is on bench. Null otherwise.
    public Dictionary<Trait, int> TraitStages = new();
    public List<int> _Stages = new();

    public int StarLevel;
    public int AttackDamage;

    public readonly int MAX_STAR_LEVEL = 3;
    [SerializeField] public Player Owner { get => _owner; set => _owner = value; }
    public int MaxHp { get => _maxHp; private set => _maxHp = value; }
    public int MaxMp { get => _maxMp; private set => _maxMp = value; }
    public int BaseAttackDamage { get => _baseAttackDamage; private set => _baseAttackDamage = value; }
    public bool IsOnBoard { get => _isOnBoard; set => _isOnBoard = value; }
    public Hex CurrentHex { get => _currentHex; set => _currentHex = value; }
    public BenchSlot CurrentBenchSlot { get => _currentBenchSlot; set => _currentBenchSlot = value; }
    public int Range { get => _range; set => _range = value; }

    private void Awake()
    {
        Transform animationTransform = transform.Find("Animation");
        if (animationTransform != null)
        {
            _animator = animationTransform.GetComponent<Animator>();
        }
        else
        {
            Debug.LogWarning("Missing animation transform on game unit " + UnitName);
        }
    }

    private void Start()
    {
        AttackDamage = _baseAttackDamage;

        // Initialize all traits with stage 0
        foreach (var trait in Traits)
        {
            TraitStages.Add(trait, 0);
        }

        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    public void Initialize(Player owner, UnitData unitData, int starLevel)
    {
        _owner = owner;
        SetUnitData(unitData);
        SetUnitStarLevel(starLevel);
    }


    public override void SetUnitData(UnitData unitData)
    {
        base.SetUnitData(unitData);
        _maxHp = _unitData.MaxHp;
        _maxMp = _unitData.MaxMp;
        _baseAttackDamage = _unitData.BaseAttackDamage;
        Range = _unitData.Range;
    }
    private void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                break;
            case GamePhase.Battle:
                _currentGamePhase = newPhase;
                break;
            case GamePhase.BattleWon:
            case GamePhase.BattleLost:
                break;
        }
    }

    public void HandleDragStarted()
    {
        Shop.Instance.ActivateUnitSellField(Cost);
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
                    _owner.Bench.PutUnitOnBenchSlot(this, benchSlotDraggedOn);
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
            Debug.Log("Units limit reached");
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
            _owner.Bench.PutUnitOnBenchSlot(this, _currentBenchSlot);
        }
    }

    // Create star objects and set current star level
    public void SetUnitStarLevel(int starLevel)
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
        StarLevel = starLevel;
    }

    public bool Equals(GameUnit other)
    {
        return UnitName == other.UnitName;
    }

    public void PlaceOnBenchSlot(BenchSlot benchSlot)
    {
        _currentBenchSlot = benchSlot;
        benchSlot.IsTaken = true;
        transform.position = _currentBenchSlot.transform.position;
        transform.SetParent(benchSlot.transform);
    }

    public void RemoveFromBench()
    {
        if (_currentBenchSlot != null)
        {
            _currentBenchSlot.IsTaken = false;
            _currentBenchSlot = null;
        }
    }

    public void PlaceOnHex(Hex hex)
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
        _currentHex = hex;
        hex.IsTaken = true;
        hex.UnitOnHex = this;
        _isOnBoard = true;
        transform.SetParent(hex.transform);
        transform.position = hex.transform.position;
    }

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

    public void Attack(GameUnit target)
    {
        if (_animator != null)
        {
            _animator.SetBool("Cast0", true);
        }
        else
        {
            Debug.LogWarning("Missing animator");
        }
        //animator.SetTrigger("Cast");
    }
    public void StopAttack(GameUnit target)
    {
        if (_animator != null)
        {
            //_animator.SetBool("Cast0", false);
        }
        else
        {
            Debug.LogWarning("Missing animator");
        }
    }

    public void UseAbility()
    {
        // Check if the unit has an ability and execute it
        //Ability?.ExecuteAbility();
    }

}
