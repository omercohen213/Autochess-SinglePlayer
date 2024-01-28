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

    private Hex _currentHex; // Current hex spot if unit is on board. Null otherwise

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
            //  Place on board
            if (IsDraggedOnBoard(finalPosition))
            {
                Board.Instance.PlaceUnitOnBoard(this, _currentHex);
            }

            // Sell unit
            else if (IsDraggedOnSellField(finalPosition))
            {
                Shop.Instance.SellUnit(this);
                Destroy(gameObject);
            }

            //else if (isDraggedOnBenchSpot()){


            // Return to its bench position
            else
            {
                ReturnToBench();
            }

            Shop.Instance.DisableUnitSellField();
        }
    }

    // Checks if the unit collides with the unit sell field
    private bool IsDraggedOnSellField(Vector3 finalPosition)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Shop"); // Create a layer mask to ignore the layer of the BoardUnit      
        RaycastHit2D hit = Physics2D.Raycast(finalPosition, Vector2.zero, Mathf.Infinity, layerMask); // Perform the raycast with the layer mask
        if (hit.collider != null && hit.collider.gameObject == Shop.Instance.UnitSellField)
        {
            return true;
        }
        return false;
    }

    // Checks if the unit collides with a board hex and assigns to current hex
    private bool IsDraggedOnBoard(Vector3 finalPosition)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Board"); // Create a layer mask to ignore the layer of the BoardUnit   
        RaycastHit2D hit = Physics2D.Raycast(finalPosition, Vector2.zero, Mathf.Infinity, layerMask); // Convert the finalPosition to screen space

        if (hit.collider != null && hit.collider.CompareTag("Hex"))
        {
            _currentHex = hit.collider.gameObject.GetComponent<Hex>();
            return true;
        }
        return false;
    }

    private void ReturnToBench()
    {
        if (_currentBenchSlot != null)
        {
            transform.position = _currentBenchSlot.transform.position;
        }
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
