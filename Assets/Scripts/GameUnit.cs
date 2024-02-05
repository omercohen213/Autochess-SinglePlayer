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
    public List<int> Stages = new();
    public int starLevel;

    public Player Owner { get => _owner; private set => _owner = value; }
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
        starLevel = 1;
        Instantiate(_starPrefab, _starsParent);
    }

    public void HandleDragStarted()
    {
        Shop.Instance.ActivateUnitSellField();
    }

    // Handles a behavior when this unit is stopped being dragged at final position
    public void HandleDragStopped(GameObject objDraggedOn)
    {
        string objTag = objDraggedOn != null ? objDraggedOn.tag : null;

        // The unit was not dragged on a game object - return unit to its current bench or hex
        if (objTag == null)
        {
            if (IsOnBoard)
            {
                Board.Instance.PlaceUnitOnBoard(this, _currentHex);
            }
            else
            {
                _owner.Bench.PutUnitOnBenchSlot(this, _currentBenchSlot);
            }
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
                    Hex hexDraggedOn = objDraggedOn.GetComponent<Hex>();
                    Board.Instance.PlaceUnitOnBoard(this, hexDraggedOn);
                    break;

                // Place on another bench slot
                case "BenchSlot":
                    BenchSlot benchSlotDraggedOn = objDraggedOn.GetComponent<BenchSlot>();
                    _owner.Bench.PutUnitOnBenchSlot(this, benchSlotDraggedOn);
                    break;
                // Return unit to its current bench or hex
                default:
                    if (IsOnBoard)
                    {
                        Board.Instance.PlaceUnitOnBoard(this, CurrentHex);
                    }
                    else
                    {
                        _owner.Bench.PutUnitOnBenchSlot(this, _currentBenchSlot);
                    }
                    break;
            }

        }
        Shop.Instance.DisableUnitSellField();
    }

    public void StarUp()
    {
        Instantiate(_starPrefab, _starsParent);
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
