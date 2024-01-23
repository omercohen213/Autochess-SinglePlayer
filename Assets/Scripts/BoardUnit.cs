using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardUnit : MonoBehaviour
{
    [SerializeField] private string unitName;
    public string UnitName { get => unitName; private set => unitName = value; }

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
        DragEvents.OnDragStopped += HandleDragStopped;
    }

    private void OnDisable()
    {
        DragEvents.OnDragStopped -= HandleDragStopped;
    }

    // Implement behavior for this BoardUnit when dragging stops
    private void HandleDragStopped(GameObject sender, Vector3 finalPosition)
    {
        if (sender == gameObject)
        {
            Debug.Log($"Unit at {transform.position} was dragged to {finalPosition}");
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
    public void SetUnitData(UnitData unitData)
    {
        UnitName = unitData.UnitName;
        Hp = unitData.Hp;
        Mp = unitData.Mp;
        AttackDamage = unitData.AttackDamage;
        UnitSprite = unitData.UnitSprite;
    }
}
