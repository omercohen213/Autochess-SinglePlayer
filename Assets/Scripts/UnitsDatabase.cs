using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitsDatabase : ScriptableObject
{
    public List<UnitData> UnitDatas;

    public UnitData FindUnitByName(string name)
    {
        UnitData unitData = UnitDatas.Find(unit => unit.UnitName == name);
        if (unitData != null)
        {
            return unitData;

        }
        else
        {
            Debug.LogWarning("Unit data " + name + " was not found");
            return null;
        }

    }
}
