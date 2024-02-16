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

    private List<Hex> _hexes = new();

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
                Hex hex = hexGo.GetComponent<Hex>();
                hex.Initialize(i,j);
                _hexes.Add(hex);
                
                pos += new Vector3(0f, HEX_SPACING_Y); // Space out the hexes in the Y axis
            }
            pos = new Vector3(pos.x + HEX_SPACING_X, transform.position.y); // Space out in the X axis           
        }
    }

    public Hex GetHex(int x, int y)
    {
        foreach(Hex hex in _hexes)
        {
            if (hex.X == x && hex.Y == y)
            {
                return hex;
            }
        }
        return null;
    }

    // Place the unit on an hex on board
    public void PlaceUnitOnBoard(GameUnit gameUnit, Hex hex)
    {
        // Unit is already on board
        if (gameUnit.IsOnBoard)
        {
            // Hex is not taken - place the unit on it
            if (!hex.IsTaken)
            {
                gameUnit.PlaceOnHex(hex);              
            }
            // Hex is taken, return unit to its current hex
            else
            {
                gameUnit.PlaceOnHex(gameUnit.CurrentHex);
            }
        }
        // Unit is on bench
        else
        {
            // Hex is taken, return it to its current bench slot
            if (hex.IsTaken)
            {
                gameUnit.Owner.Bench.PutUnitOnBenchSlot(gameUnit, gameUnit.CurrentBenchSlot);
            }
            // Hex is not taken, Remove it from bench and add it to board on given hex
            else
            {
                gameUnit.Owner.Bench.RemoveUnitFromBench(gameUnit);
                gameUnit.Owner.BoardUnits.Add(gameUnit);
                gameUnit.PlaceOnHex(hex);                
                UpdateBoardTraits(gameUnit);
                UIManager.Instance.UpdateBoardLimit();
            }
        }
    }

    // Remove a unit from board
    public void RemoveUnitFromBoard(GameUnit unit)
    {
        unit.RemoveFromBoard();       
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
