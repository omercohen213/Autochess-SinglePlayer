using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Bench : MonoBehaviour
{
    [SerializeField] private List<GameUnit> _units; // Units on bench
    [SerializeField] private GameObject _gameUnitPrefab;
    [SerializeField] private Transform _benchSlotPrefab;


    private readonly int BENCH_SIZE = 10;
    private readonly int BENCH_COLUMNS = 2;
    private readonly float BENCHSLOT_SPACING = -1.2f; // Space between two benchSlots in the Y axis
    private readonly int UNIT_STAR_UP_COUNT = 3;

    public void Awake()
    {
        _units = new List<GameUnit>();
    }

    private void Start()
    {
        CreateBench();
    }

    // Create bench and bench slot objects
    private void CreateBench()
    {
        Vector3 pos = transform.position; // Set initial benchSlot position
        for (int i = 0; i < BENCH_COLUMNS; i++)
        {
            for (int j = 0; j < BENCH_SIZE / BENCH_COLUMNS; j++)
            {
                // Create benchSlot
                Transform benchSlotTransform = Instantiate(_benchSlotPrefab, pos, Quaternion.identity, transform);
                benchSlotTransform.gameObject.AddComponent<BenchSlot>();
                benchSlotTransform.GetComponent<BenchSlot>().SetPosition(i, j);
                benchSlotTransform.gameObject.name = $"BenchSlot ({i},{j})";
                pos += new Vector3(0f, BENCHSLOT_SPACING); // Space out the benchSlots in the Y axis
            }
            pos += new Vector3(BENCHSLOT_SPACING, 5f); // Space out in the X axis and Y axis for the second column
        }
    }

    // Create an instance of a unit on scene and set its data according to the id
    public void CreateGameUnit(int id, int starLevel)
    {
        GameObject unitGo = Instantiate(_gameUnitPrefab, transform);
        GameUnit gameUnit = unitGo.GetComponent<GameUnit>();
        gameUnit.SetUnitData(id);
        SpriteRenderer unitSpriteRenderer = gameUnit.GetComponent<SpriteRenderer>();
        unitSpriteRenderer.sprite = gameUnit.UnitSprite;
        AddUnitToBench(gameUnit);
        gameUnit.StarUp();
        CheckStarUp(gameUnit, starLevel);
    }

    // Create a gameUnit and add it to the bench
    public void AddUnitToBench(GameUnit unit)
    {
        _units.Add(unit);
        BenchSlot benchSlot = FindEmptyBenchSlot();
        benchSlot.IsTaken = true;
        unit.CurrentBenchSlot = benchSlot;
        unit.transform.SetParent(benchSlot.transform);
        unit.transform.position = benchSlot.transform.position;
    }

    // Check if there are enough of the same unit on bench to star it up
    private void CheckStarUp(GameUnit unit, int starLevel)
    {
        int sameUnitsCount = SameUnitsCount(unit);
        if (sameUnitsCount >= UNIT_STAR_UP_COUNT)
        {
            RemoveAllUnitsOfKind(unit);
            CreateGameUnit(unit.UnitData.Id, starLevel+1);
        }        
    }

    private void RemoveAllUnitsOfKind(GameUnit unit)
    {
        List<GameUnit> unitsToRemove = new List<GameUnit>();

        // Collect units to remove
        foreach (GameUnit gameUnit in _units)
        {
            if (gameUnit.Equals(unit))
            {
                unitsToRemove.Add(gameUnit);
            }
        }

        // Remove collected units
        foreach (GameUnit unitToRemove in unitsToRemove)
        {
            RemoveUnitFromBench(unitToRemove);
            Destroy(unitToRemove.gameObject);
        }
    }

    // Returns the count of same units on bench as unit
    private int SameUnitsCount(GameUnit unit)
    {
        int count = 0;
        foreach (GameUnit gameUnit in _units)
        {
            if (gameUnit.Equals(unit))
            {
                count++;
            }
        }
        return count;
    }


    // Returns the first empty benchSlot 
    private BenchSlot FindEmptyBenchSlot()
    {
        // Go over the bench slots
        for (int i = 0; i < BENCH_SIZE; i++)
        {
            if (transform.GetChild(i).TryGetComponent<BenchSlot>(out var benchSlot))
            {
                // Check if there is a unit in slot
                if (!benchSlot.IsTaken)
                {
                    return benchSlot;
                }
            }
        }
        return null;
    }

    // Change the bench slot of unit to given one
    public void PutUnitOnBenchSlot(GameUnit unit, BenchSlot benchSlot)
    {
        // Bench slot is taken
        if (benchSlot.IsTaken)
        {
            // Unit is on board- return it to its current hex on board
            if (unit.IsOnBoard)
            {
                Board.Instance.PlaceUnitOnBoard(unit, unit.CurrentHex);
                return;
            }
            // Unit is already on bench- return it to its current bench slot
            else
            {
                benchSlot = unit.CurrentBenchSlot;
            }
        }

        // Bench slot is empty
        else
        {
            // Unit is on board- remove it from board and add to bench on given bench slot
            if (unit.IsOnBoard)
            {
                Board.Instance.RemoveUnitFromBoard(unit);
                _units.Add(unit);
            }
            // Unit is already on bench- change its bench slot to given one
            else
            {
                unit.CurrentBenchSlot.IsTaken = false;
            }
            unit.CurrentBenchSlot = benchSlot;
            benchSlot.IsTaken = true;
        }

        // Change to the given bench slot
        unit.transform.position = unit.CurrentBenchSlot.transform.position;
        unit.transform.SetParent(benchSlot.transform);
    }

    // Check if bench exceeds max units limit
    public bool IsFull()
    {
        if (_units.Count >= BENCH_SIZE)
        {
            Debug.Log("Units bench is full!");
            return true;
        }
        return false;
    }

    // Remove a unit from the bench
    public void RemoveUnitFromBench(GameUnit unit)
    {
        if (unit.CurrentBenchSlot != null)
        {
            unit.CurrentBenchSlot.IsTaken = false;
            unit.CurrentBenchSlot = null;
        }
        _units.Remove(unit);
    }

    // Print all unit names
    public override string ToString()
    {
        string str = "";
        foreach (GameUnit unit in _units)
        {
            str += unit.UnitName;
        }
        return str;

    }
}
