using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bench : MonoBehaviour
{

    [SerializeField] private List<BoardUnit> _benchUnits; // Units on bench
    [SerializeField] private Transform _benchTransform;
    [SerializeField] private GameObject _boardUnitPrefab; // For the instantiation when adding to bench

    private readonly int BENCH_SIZE = 10;

    public void Awake()
    {
        _benchUnits = new List<BoardUnit>();
    }

    // Create a boardUnit and add it to the bench
    public void AddUnitToBench(int unitId)
    {
        BoardUnit unit = BoardUnit.CreateBoardUnit(_boardUnitPrefab, _benchTransform, unitId);
        _benchUnits.Add(unit);
        ShowUnitOnBench(unit);
    }

    // Show the unit visually on the bench
    private void ShowUnitOnBench(BoardUnit unit)
    {
        Transform emptyBenchSlot = FindEmptyBenchSlot();
        if (emptyBenchSlot != null)
        {
            SpriteRenderer unitSpriteRenderer = unit.GetComponent<SpriteRenderer>();
            unitSpriteRenderer.sprite = unit.UnitSprite;
            unit.transform.SetParent(emptyBenchSlot);
            unit.transform.position = emptyBenchSlot.position;
        }
    }

    // Returns an empty bench slot transform
    private Transform FindEmptyBenchSlot()
    {
        // Go over the bench slots
        for (int i = 0; i < BENCH_SIZE; i++)
        {
            string benchSlotName = "BenchSlot";
            GameObject benchSlotGo = GameObject.Find(benchSlotName + (i + 1));

            if (benchSlotGo != null)
            {
                // Check if there is a unit in slot
                if (benchSlotGo.transform.childCount == 0)
                {
                    return benchSlotGo.transform;
                }
            }
        }
        return null;
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
        unit.DestroyUnit();
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
}
