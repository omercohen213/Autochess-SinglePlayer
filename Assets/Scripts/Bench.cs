using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bench : MonoBehaviour
{
    [SerializeField] private List<GameUnit> _benchUnits;
    [SerializeField] private GameObject _gameUnitPrefab;
    [SerializeField] private Transform _benchSlotPrefab;

    private readonly int BENCH_SIZE = 10;
    private readonly int BENCH_COLUMNS = 2;
    private readonly float BENCHSLOT_SPACING = -1.5f; // Space between two benchSlots in the Y axis
    private readonly int UNIT_STAR_UP_COUNT = 3;

    public List<GameUnit> BenchUnits { get => _benchUnits; set => _benchUnits = value; }

    public void Awake()
    {
        _benchUnits = new List<GameUnit>();
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
    public void CreateEnemyUnitByName(string unitName, int starLevel)
    {
        UnitData unitData = GameUnitsDatabase.Instance.FindUnitByName(unitName);
        GameObject unitGo = Instantiate(unitData.UnitPrefab);
        if (unitGo.TryGetComponent(out GameUnit gameUnit))
        {
            gameUnit.Initialize(Opponent.Instance, unitData, starLevel);
        }
        else
        {
            Debug.LogWarning("Missing gameUnit component");
        }
        AddUnitToBench(gameUnit);
    }

    // Create an instance of a unit on scene and initialize it
    public void CreateGameUnit(Player owner, UnitData unitData, int starLevel)
    {
        GameObject unitGo = Instantiate(unitData.UnitPrefab);
        if (unitGo.TryGetComponent(out GameUnit gameUnit))
        {
            gameUnit.Initialize(owner, unitData, starLevel);
        }
        else
        {
            Debug.LogWarning("Missing gameUnit component");
        }
        AddUnitToBench(gameUnit);
    }

    // Add a unit to an empty bench slot
    public void AddUnitToBench(GameUnit gameUnit)
    {
        _benchUnits.Add(gameUnit);
        BenchSlot benchSlot = FindEmptyBenchSlot();
        gameUnit.PlaceOnBenchSlot(benchSlot);
        CheckStarUp(gameUnit, gameUnit.StarLevel);
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
            if (starLevel + 1 == GameUnit.MAX_STAR_LEVEL)
            {
                Shop.Instance.RemoveUnitFromShopDB(gameUnit);
            }
        }
    }

    // Remove all units same as unit with same star level
    private void RemoveAllUnitsOfKind(GameUnit gameUnit, int starLevel)
    {
        List<GameUnit> unitsToRemove = new();

        // Bench units
        foreach (GameUnit benchUnit in _benchUnits)
        {
            if (benchUnit.Equals(gameUnit) && benchUnit.StarLevel == starLevel)
            {
                unitsToRemove.Add(benchUnit);
            }
        }

        // Board units
        foreach (GameUnit boardUnit in gameUnit.Owner.BoardUnits)
        {
            if (boardUnit.Equals(gameUnit) && boardUnit.StarLevel == starLevel)
            {
                unitsToRemove.Add(boardUnit);
            }
        }

        // Remove collected units
        foreach (GameUnit unitToRemove in unitsToRemove)
        {
            RemoveUnitFromBench(unitToRemove);
            Board.RemoveUnitFromBoard(unitToRemove);
            Destroy(unitToRemove.gameObject);
        }
    }

    // Returns the count of same units and same star level as unit
    private int SameUnitsCount(GameUnit gameUnit)
    {
        int count = 0;

        // Bench Units
        foreach (GameUnit benchUnit in _benchUnits)
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
    public void PlaceOnBenchSlot(GameUnit gameUnit, BenchSlot benchSlot)
    {
        // Bench slot is taken
        if (benchSlot.IsTaken)
        {
            // Unit is on board- swap units between benchslot and hex
            if (gameUnit.IsOnBoard)
            {
                Hex currentHex = gameUnit.CurrentHex;
                Board.RemoveUnitFromBoard(gameUnit);
                Board.PlaceUnitOnBoard(benchSlot.UnitOnSlot, currentHex);
                gameUnit.PlaceOnBenchSlot(benchSlot);
                return;
            }
            // Unit is on bench - swap places
            else if (gameUnit.CurrentBenchSlot != null)
            {
                GameUnit unitOnSlot = benchSlot.UnitOnSlot;
                if (unitOnSlot != null && unitOnSlot != gameUnit)
                {
                    unitOnSlot.RemoveFromBench();
                    unitOnSlot.PlaceOnBenchSlot(gameUnit.CurrentBenchSlot);
                }
                gameUnit.PlaceOnBenchSlot(benchSlot);
            }
        }

        // Bench slot is empty
        else
        {
            // Unit is on board- remove it from board and add to bench on given bench slot
            if (gameUnit.IsOnBoard)
            {
                _benchUnits.Add(gameUnit);
                benchSlot.UnitOnSlot = gameUnit;
                Board.RemoveUnitFromBoard(gameUnit);
                UIManager.Instance.UpdateBoardLimit();
            }

            // Unit is on bench - swap units between benchslots
            else if (gameUnit.CurrentBenchSlot != null)
            {
                gameUnit.RemoveFromBench();
            }
            gameUnit.PlaceOnBenchSlot(benchSlot);
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
    public void RemoveUnitFromBench(GameUnit gameUnit)
    {
        gameUnit.RemoveFromBench();
        _benchUnits.Remove(gameUnit);
    }

    // Print all unit names
    public override string ToString()
    {
        string str = "";
        foreach (GameUnit unit in _benchUnits)
        {
            str += unit.UnitName;
        }
        return str;

    }
}
