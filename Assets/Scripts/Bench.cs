using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bench : MonoBehaviour
{

    [SerializeField] private List<BoardUnit> _benchUnits; // Units on bench
    [SerializeField] private Transform _benchTransform;
    [SerializeField] private GameObject _unitPrefab;

    private readonly int BENCH_SIZE = 9;

    public void Awake()
    {
        _benchUnits = new List<BoardUnit>();
    }

    // Add a unit to the bench
    public void AddUnitToBench(ShopUnit shopUnit)
    {
        BoardUnit unit = CreateUnitOnBoard(shopUnit);
        _benchUnits.Add(unit);
        ShowUnitOnBench(unit);
    }

    // Show the unit in the bench visually
    private void ShowUnitOnBench(BoardUnit unit)
    {
        /*for (int i = 0; i < _benchTransform.childCount; i++)
        {
            GameObject benchUnitGo = _benchTransform.GetChild(i).GetChild(0).gameObject;
            if (!benchUnitGo.activeSelf)
            {
                SpriteRenderer unitSpriteRenderer = benchUnitGo.GetComponent<SpriteRenderer>();
                unitSpriteRenderer.sprite = unit.UnitSprite;
                Unit benchUnit = unitSpriteRenderer.GetComponent<Unit>();
                benchUnit.SetUnitData(unit);
                benchUnitGo.SetActive(true);
                return;
            }
        }*/

        for (int i = 0; i < _benchTransform.childCount; i++)
        {
            Transform benchUnitParent = _benchTransform.GetChild(i);
            if (benchUnitParent.childCount == 0)
            {
                SpriteRenderer unitSpriteRenderer = unit.GetComponent<SpriteRenderer>();
                unitSpriteRenderer.sprite = unit.UnitSprite;
                unit.transform.SetParent(benchUnitParent);
                unit.transform.position = benchUnitParent.position;
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
    public void RemoveUnit(BoardUnit unit)
    {
        _benchUnits.Remove(unit);
    }

    // Print all unit names
    public string PrintUnitsOnBench()
    {
        string str = "";
        foreach (BoardUnit unit in _benchUnits)
        {
            str += unit.UnitName;
        }
        return str;
    }

    public BoardUnit CreateUnitOnBoard(ShopUnit shopUnit)
    {
        GameObject unitGo = Instantiate(_unitPrefab, _benchTransform);
        BoardUnit unit = unitGo.GetComponent<BoardUnit>();
        unit.SetUnitData(shopUnit.UnitData);
        return unit;
    }
}
