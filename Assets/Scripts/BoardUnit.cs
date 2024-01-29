using System.Net.Http.Headers;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BoardUnit : Unit
{
    [SerializeField] private int hp;
    public int Hp { get => hp; private set => hp = value; }

    [SerializeField] private int mp;
    public int Mp { get => mp; private set => mp = value; }

    [SerializeField] private int attackDamage;
    public int AttackDamage { get => attackDamage; private set => attackDamage = value; }

    [SerializeField] private Sprite unitSprite;
    public Sprite UnitSprite { get => unitSprite; private set => unitSprite = value; }

    private BenchSlot _currentBenchSlot; // Bench spot if unit is on bench. Null otherwise.
    public BenchSlot CurrentBenchSlot { get => _currentBenchSlot; set => _currentBenchSlot = value; }

    private Player _owner;
    private Hex _currentHex; // Current hex spot if unit is on board. Null otherwise

    private void Start()
    {
        _owner = Player.Instance; // TEMPORARY
    }

    private void OnEnable()
    {
        DragEvents.OnDragStarted += HandleDragStarted;
        DragEvents.OnDragStopped += HandleDragStopped;
    }

    private void OnDisable()
    {
        DragEvents.OnDragStarted += HandleDragStarted;
        DragEvents.OnDragStopped -= HandleDragStopped;
    }

    private void HandleDragStarted(GameObject sender)
    {
        Shop.Instance.ActivateUnitSellField();
    }

    // Handles a behavior when this unit is stopped being dragged at final position
    private void HandleDragStopped(GameObject sender, Vector3 finalPosition)
    {
        if (sender == gameObject)
        {
            GameObject objDraggedOn = GetDragPosition(finalPosition);
            string objTag = objDraggedOn != null ? objDraggedOn.tag : null;
            if (objTag == null)
            {
                _owner.UnitsBench.ReturnUnitToBench(this);
                Debug.Log("nl");
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
                        _owner.UnitsBench.RemoveUnitFromBench(this);
                        Board.Instance.PlaceUnitOnBoard(this, hexDraggedOn);
                        break;

                    // Place on another bench slot
                    case "BenchSlot":
                        Debug.Log("ss");
                        BenchSlot benchSlotDraggedOn = objDraggedOn.GetComponent<BenchSlot>();
                        _owner.UnitsBench.ChangeBenchSlot(this, benchSlotDraggedOn);
                        break;
                    // Return to bench
                    default:
                        _owner.UnitsBench.ReturnUnitToBench(this);
                        break;
                }
            }
        }
        Shop.Instance.DisableUnitSellField();
    }

    private GameObject GetDragPosition(Vector3 finalPosition)
    {
        int shopLayer = LayerMask.NameToLayer("Shop");
        int boardLayer = LayerMask.NameToLayer("Board");
        int benchLayer = LayerMask.NameToLayer("Bench");

        int layerMask = (1 << shopLayer) | (1 << boardLayer) | (1 << benchLayer); // Create a layer mask to ignore the BoardUnit collider
        RaycastHit2D hit = Physics2D.Raycast(finalPosition, Vector2.zero, Mathf.Infinity, layerMask); // Perform the raycast with the layer mask
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public void Attack(BoardUnit target)
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
        Hp = UnitData.Hp;
        Mp = UnitData.Mp;
        AttackDamage = UnitData.AttackDamage;
        UnitSprite = UnitData.Sprite;
    }
}
