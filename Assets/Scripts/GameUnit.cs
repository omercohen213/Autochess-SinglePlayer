using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameUnit : Unit
{
    private Player _owner;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxMp;
    [SerializeField] private int _baseAttackDamage;
    [SerializeField] private Sprite _unitSprite;
    [SerializeField] private GameObject _starPrefab;
    [SerializeField] private Transform _starsParent;
    public int AttackDamage; //{ get => _attackDamage; set => _attackDamage = value; }
    public bool _isOnBoard;
    public Hex _currentHex; // Current hex spot if unit is on board. Null otherwise
    public BenchSlot _currentBenchSlot; // Bench spot if unit is on bench. Null otherwise.
    public Dictionary<Trait, int> TraitStages = new();
    public List<int> _Stages = new();
    public int StarLevel;
    public readonly int MAX_STAR_LEVEL = 3;

    public Player Owner { get => _owner; set => _owner = value; }
    public int MaxHp { get => _maxHp; private set => _maxHp = value; }
    public int MaxMp { get => _maxMp; private set => _maxMp = value; }
    public int BaseAttackDamage { get => _baseAttackDamage; private set => _baseAttackDamage = value; }
    public Sprite UnitSprite { get => _unitSprite; private set => _unitSprite = value; }
    public bool IsOnBoard { get => _isOnBoard; set => _isOnBoard = value; }
    public Hex CurrentHex { get => _currentHex; set => _currentHex = value; }
    public BenchSlot CurrentBenchSlot { get => _currentBenchSlot; set => _currentBenchSlot = value; }


    private void Start()
    {
        _owner = LocalPlayer.Instance;
        AttackDamage = _baseAttackDamage;

        // Initialize all traits with stage 0
        foreach (var trait in Traits)
        {
            TraitStages.Add(trait, 0);
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
        for (int i = 0; i < starLevel; i++)
        {
            Instantiate(_starPrefab, _starsParent);
        }
        StarLevel = starLevel;

        // *** override in child gameunit for unique behavior
    }

    public void Attack(GameUnit target)
    {
        // Implement attack logic
    }

    public void UseAbility()
    {
        // Check if the unit has an ability and execute it
        //Ability?.ExecuteAbility();
    }

    // Set the unit's properties based on unitData
    public override void SetUnitData(int id)
    {
        base.SetUnitData(id);
        MaxHp = UnitData.MaxHp;
        MaxMp = UnitData.MaxMp;
        BaseAttackDamage = UnitData.BaseAttackDamage;
        UnitSprite = UnitData.Sprite;
    }

    public bool Equals(GameUnit other)
    {
        return UnitData.Id == other.UnitData.Id;
    }
}
