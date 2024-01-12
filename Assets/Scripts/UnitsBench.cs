using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitsBench : MonoBehaviour
{

    [SerializeField] private List<Unit> _benchUnits;
    [SerializeField] private Transform _benchTransform;

    private readonly int BENCH_SIZE = 9;

    public void Awake()
    {
        _benchUnits = new List<Unit>();
    }

    // Add a unit to the bench
    public void AddUnit(Unit unitToAdd)
    {
        _benchUnits.Add(unitToAdd);
        ShowUnitOnBench(unitToAdd);
    }

    private void ShowUnitOnBench(Unit unit)
    {
        for (int i = 0; i < _benchTransform.childCount; i++)
        {
            GameObject benchUnitGo = _benchTransform.GetChild(i).GetChild(0).gameObject;
            if (!benchUnitGo.activeSelf)
            {
                SpriteRenderer unitSpriteRenderer = benchUnitGo.GetComponent<SpriteRenderer>();
                unitSpriteRenderer.sprite = unit.UnitSprite;
                benchUnitGo.SetActive(true);
                return;
            }
        }

    }

    // Check if bench exceeds max units limit
    public bool IsFull()
    {
        if (_benchUnits.Count >= BENCH_SIZE)
        {
            Debug.Log("Units bench is full!");
            return true;
        }
        return false;
    }

    // Remove a unit from the bench
    public void RemoveUnit(Unit unit)
    {
        _benchUnits.Remove(unit);
    }

    // Print all unit names
    public string PrintUnits()
    {
        string str = "";
        foreach (Unit unit in _benchUnits)
        {
            str += unit.UnitName;
        }
        return str;
    }
}
