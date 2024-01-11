using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsBench : MonoBehaviour
{
    [SerializeField] private List<Unit> _units;

    public void Awake()
    {
        _units = new List<Unit>();       
    }

    // Access the units list
    public List<Unit> Units
    {
        get { return _units; }
    }

    // Add a unit to the bench
    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }

    // Remove a unit from the bench
    public void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
    }

    // Print all unit names
    public string PrintUnits()
    {
        string str = "";
        foreach (Unit unit in _units)
        {
            str += unit.UnitName;
        }
        return str;
    }
}
