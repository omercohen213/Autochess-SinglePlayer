using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bench : MonoBehaviour
{
    [SerializeField] private List<GameUnit> _units; // Units on bench
    [SerializeField] private GameObject _gameUnitPrefab;
    [SerializeField] private Transform _benchSlotPrefab;

    private readonly int BENCH_SIZE = 10;
    private readonly int BENCH_COLUMNS = 2;
    private readonly float BENCHSLOT_SPACING = -1.5f; // Space between two benchSlots in the Y axis
    private readonly int UNIT_STAR_UP_COUNT = 3;

    public List<GameUnit> BenchUnits { get => _units; set => _units = value; }

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
            pos += new Vector3(BENCHSLOT_SPACING, 7f); // Space out in the X axis and Y axis for the second column
        }
    }

    // Create an instance of a unit on scene and initialize it
    public void CreateGameUnitByName(Player owner, string unitName, int starLevel)
    {
        UnitData unitData = GameUnitsDatabase.Instance.FindUnitByName(unitName);
        GameObject unitGo = Instantiate(unitData.UnitPrefab);
        if (unitGo.TryGetComponent(out GameUnit gameUnit))
        {
            gameUnit.Initialize(owner,unitData, starLevel);
        }
        else
        {
            Debug.LogWarning("Missing gameUnit component");
        }
        AddUnitToBench(gameUnit);
    }
   
    // Create the gameUnit object
    public void CreateGameUnit(Player owner, UnitData unitData, int starLevel)
    {
        GameObject unitGo = Instantiate(unitData.UnitPrefab);
        if (unitGo.TryGetComponent(out GameUnit gameUnit))
        {
            gameUnit.Initialize(owner,unitData, starLevel);
        }
        else
        {
            Debug.LogWarning("Missing gameUnit component");
        }
        AddUnitToBench(gameUnit);
    }

    // Create a gameUnit and add it to the bench
    public void AddUnitToBench(GameUnit unit)
    {
        _units.Add(unit);
        BenchSlot benchSlot = FindEmptyBenchSlot();
        unit.PlaceOnBenchSlot(benchSlot);
        CheckStarUp(unit, unit.StarLevel);
    }

    // Check if there are enough of the same unit to star it up
    private void CheckStarUp(GameUnit gameUnit, int starLevel)
    {
        int sameUnitsCount = SameUnitsCount(gameUnit);
        if (sameUnitsCount >= UNIT_STAR_UP_COUNT)
        {
            RemoveAllUnitsOfKind(gameUnit, starLevel);
            CreateGameUnit(gameUnit.Owner, gameUnit.UnitData, starLevel + 1);

            // If unit reaches max star level, remove from database
            if (starLevel + 1 == gameUnit.MAX_STAR_LEVEL)
            {
                Shop.Instance.RemoveUnitFromShopDB(gameUnit);
            }
        }
    }

    // Remove all units same as unit with same star level
    private void RemoveAllUnitsOfKind(GameUnit unit, int starLevel)
    {
        List<GameUnit> unitsToRemove = new();

        // Bench units
        foreach (GameUnit benchUnit in _units)
        {
            if (benchUnit.Equals(unit) && benchUnit.StarLevel == starLevel)
            {
                unitsToRemove.Add(benchUnit);
            }
        }

        // Board units
        foreach (GameUnit boardUnit in unit.Owner.BoardUnits)
        {
            if (boardUnit.Equals(unit) && boardUnit.StarLevel == starLevel)
            {
                unitsToRemove.Add(boardUnit);
            }
        }

        // Remove collected units
        foreach (GameUnit unitToRemove in unitsToRemove)
        {
            RemoveUnitFromBench(unitToRemove);
            Board.Instance.RemoveUnitFromBoard(unitToRemove);
            Destroy(unitToRemove.gameObject);
        }
    }

    // Returns the count of same units and same star level as unit
    private int SameUnitsCount(GameUnit gameUnit)
    {
        int count = 0;

        // Bench Units
        foreach (GameUnit benchUnit in _units)
        {
            if (benchUnit.Equals(gameUnit) && benchUnit.StarLevel == gameUnit.StarLevel)
            {
                count++;
            }
        }

        // Board units
        foreach (GameUnit boardUnit in gameUnit.Owner.BoardUnits)
        {
            if (boardUnit.Equals(gameUnit) && boardUnit.StarLevel == gameUnit.StarLevel)
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
        }

        // Bench slot is empty
        else
        {
            // Unit is on board- remove it from board and add to bench on given bench slot
            if (unit.IsOnBoard)
            {
                Board.Instance.RemoveUnitFromBoard(unit);
                _units.Add(unit);
                UIManager.Instance.UpdateBoardLimit();
            }
        }
        unit.PlaceOnBenchSlot(benchSlot);
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
        unit.RemoveFromBench();
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
