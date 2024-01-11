using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Units Database", menuName = "Game/Units Database")]
public class UnitsDatabase : ScriptableObject
{
    public List<UnitData> Units;
    public UnitData GetUnitByName(string name)
    {
        return Units.Find(unit => unit.unitName == name);
    }
}
