using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Cost { get; private set; }
    public int Hp { get; private set; }
    public int Mp { get; private set; }
    public int AttackDamage { get; private set; }
    public string UnitName { get; private set; }

    public void Start()
    {
    }

    public void Attack(Unit target)
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
        UnitName = unitData.unitName;
        Cost = unitData.cost;
        Hp = unitData.hp;
        Mp = unitData.mp;
        AttackDamage = unitData.attackDamage;
    }
}
