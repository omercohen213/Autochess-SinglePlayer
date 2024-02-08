using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject _hexPrefab;
    [SerializeField] private Transform _board;


    public readonly int _ROWS = 5;
    public readonly int _COLUMNS = 8;
    private readonly float HEX_SPACING_Y = -1.35f; // Space between two hexes in the Y axis
    private readonly float HEX_SPACING_X = 1.15f; // Space between two hexes in the X axis
    private readonly float COLUMN_SHIFT_Y = 0.68f; // Shift odd numbered columns in the Y axis

    public static Board Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        CreateBoard();
    }

    // Create board and hexes objects
    private void CreateBoard()
    {
        Vector3 pos = transform.position; // Set initial hex position
        for (int i = 0; i < _COLUMNS; i++)
        {
            // Shift odd numbered columns position 
            if (i % 2 == 1)
            {
                pos -= new Vector3(0, COLUMN_SHIFT_Y);
            }
            for (int j = 0; j < _ROWS; j++)
            {
                // Create the hex gameObject
                GameObject hexGo = Instantiate(_hexPrefab, pos, Quaternion.identity, transform);
                hexGo.AddComponent<Hex>();
                hexGo.GetComponent<Hex>().SetPosition(i, j);
                hexGo.name = $"Hex ({i},{j})";
                pos += new Vector3(0f, HEX_SPACING_Y); // Space out the hexes in the Y axis
            }
            pos = new Vector3(pos.x + HEX_SPACING_X, transform.position.y); // Space out in the X axis           
        }
    }

    // Place the unit on an hex on board
    public void PlaceUnitOnBoard(GameUnit unit, Hex hex)
    {
        // Unit is already on board
        if (unit.IsOnBoard)
        {
            // Hex is taken - return the unit to its current hex
            if (hex.IsTaken)
            {
                hex = unit.CurrentHex;
            }
            // Hex is not taken - place the unit on it
            else
            {
                unit.CurrentHex.IsTaken = false;
                hex.IsTaken = true;
                unit.CurrentHex = hex;
            }
        }
        // Unit is not on board
        else
        {
            // Hex is taken, return it to its current bench slot
            if (hex.IsTaken)
            {
                unit.Owner.Bench.PutUnitOnBenchSlot(unit, unit.CurrentBenchSlot);
            }
            // Hex is not taken, Remove it from bench and add it to board on given hex
            else
            {
                unit.Owner.Bench.RemoveUnitFromBench(unit);
                unit.Owner.BoardUnits.Add(unit);
                unit.CurrentHex = hex;
                hex.IsTaken = true;
                unit.IsOnBoard = true;
                UpdateBoardTraits(unit);
            }
        }

        unit.transform.SetParent(hex.transform);
        unit.transform.position = hex.transform.position;
    }

    // Remove a unit from board
    public void RemoveUnitFromBoard(GameUnit unit)
    {
        if (unit.CurrentHex != null)
        {
            unit.CurrentHex.IsTaken = false;
            unit.CurrentHex = null;
        }
        unit.IsOnBoard = false;
        unit.Owner.BoardUnits.Remove(unit);
        UpdateBoardTraits(unit);
    }

    // Update traits to owner's board
    private void UpdateBoardTraits(GameUnit unit)
    {
        // Update traits only if there is no same unit on board
        if (!unit.Owner.IsSameUnitOnBoard(unit))
        {
            foreach (Trait trait in unit.Traits)
            {
                List<GameUnit> unitsWithTrait = unit.Owner.GetUnitsWithTrait(trait);
                int unitCount = unitsWithTrait.Count;
                int currentTraitStage = GetBoardTraitStage(trait, unitCount);

                // Get last trait stage according to the unit addition or removal from board
                int lastTraitStage = unit.IsOnBoard ? GetBoardTraitStage(trait, unitCount - 1) : GetBoardTraitStage(trait, unitCount + 1);
                trait.UpdateTrait(unitsWithTrait, unit, currentTraitStage, lastTraitStage);
                int lastUnitCount = unit.IsOnBoard ? unitCount - 1 : unitCount + 1;
                UIManager.Instance.UpdateTraitUI(trait, currentTraitStage, unitCount, lastUnitCount);
            }
        }      
    }
    
    // Get the stage of the trait according to how many units are on board
    public int GetBoardTraitStage(Trait trait, int unitCount)
    {
        int traitStage = 0;
        for (int i = 0; i < trait.UnitNumNeeded.Length; i++)
        {
            if (unitCount >= trait.UnitNumNeeded[i])
            {
                traitStage++;
            }
        }
        return traitStage;
    }
}
