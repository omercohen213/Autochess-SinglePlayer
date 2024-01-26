using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    // Implement behavior for this BoardUnit when dragging stops
    private void HandleDragStopped(GameObject sender, Vector3 finalPosition)
    {
        if (sender == gameObject)
        {
            if (IsOnUnitSellField(gameObject, finalPosition))
            {
                Shop.Instance.SellUnit(this);
            }
            Shop.Instance.DisableUnitSellField();
        }
    }

    private bool IsOnUnitSellField(GameObject sender, Vector3 finalPosition)
    {
        // Create a layer mask to ignore the layer of the BoardUnit
        int layerMask = 1 << LayerMask.NameToLayer("Shop");

        // Perform the raycast with the layer mask
        RaycastHit2D hit = Physics2D.Raycast(finalPosition, Vector2.zero, Mathf.Infinity, layerMask);

        if (hit.collider != null && hit.collider.gameObject == Shop.Instance.UnitSellField)
        {
            return true;
        }
        return false;
    }

    // Gets a prefab and 
    public static BoardUnit CreateBoardUnit(GameObject boardUnitPrefab, Transform parent, int unitId)
    {
        GameObject unitGo = Instantiate(boardUnitPrefab, parent);
        BoardUnit boardUnit = unitGo.GetComponent<BoardUnit>();
        boardUnit.SetUnitData(unitId);
        return boardUnit;
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
        UnitSprite = UnitData.UnitSprite;
    }

    public void DestroyUnit()
    {
        Destroy(gameObject);
    }
}
