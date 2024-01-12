using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public int Cost { get; private set; }
    public int Hp { get; private set; }
    public int Mp { get; private set; }
    public int AttackDamage { get; private set; }
    public string UnitName { get; private set; }
    public Sprite UnitSprite { get; private set; }

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
        UnitName = unitData.UnitName;
        Cost = unitData.Cost;
        Hp = unitData.Hp;
        Mp = unitData.Mp;
        AttackDamage = unitData.AttackDamage;
        UnitSprite = unitData.UnitSprite;
    }
}
