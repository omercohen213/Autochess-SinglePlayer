using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                hex.Initialize(i, j);
                hex.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"({i},{j})";
                _hexes.Add(hex);

                pos += new Vector3(0f, HEX_SPACING_Y); // Space out the hexes in the Y axis
            }
            pos = new Vector3(pos.x + HEX_SPACING_X, transform.position.y); // Space out in the X axis           
        }
    }

    public Hex GetHex(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 4)
        {
            return null;
        }

        foreach (Hex hex in _hexes)
        {
            if (hex.X == x && hex.Y == y)
            {
                return hex;
            }
        }
        return null;
    }

    // Place the unit on an hex on board
    public static void PlaceUnitOnBoard(GameUnit gameUnit, Hex hex)
    {
        // Unit is already on board
        if (gameUnit.IsOnBoard)
        {
            // Hex is not taken - place the unit on it
            if (!hex.IsTaken)
            {
                gameUnit.PlaceOnHex(hex);
            }
            // Hex is taken, swap places
            else
            {
                hex.UnitOnHex.PlaceOnHex(gameUnit.CurrentHex);
                gameUnit.PlaceOnHex(hex);
            }
        }
        // Unit is on bench
        else
        {
            // Hex is taken, swap places
            if (hex.IsTaken)
            {
                GameUnit gameUnitToSwap = hex.UnitOnHex;
                BenchSlot currBenchSlot = gameUnit.CurrentBenchSlot;

                gameUnit.Owner.Bench.RemoveUnitFromBench(gameUnit);
                gameUnitToSwap.Owner.Bench.PlaceOnBenchSlot(gameUnitToSwap, currBenchSlot);
                gameUnit.Owner.BoardUnits.Add(gameUnit);
                gameUnit.PlaceOnHex(hex);
            }
            // Hex is not taken, Remove it from bench and add it to board on given hex
            else
            {
                gameUnit.Owner.Bench.RemoveUnitFromBench(gameUnit);
                gameUnit.Owner.BoardUnits.Add(gameUnit);
                gameUnit.PlaceOnHex(hex);
                UIManager.Instance.UpdateBoardLimit();
            }
            UpdateBoardTraits(gameUnit);
        }
    }

    // Remove a unit from board
    public static void RemoveUnitFromBoard(GameUnit gameUnit)
    {
        gameUnit.RemoveFromBoard();
        UpdateBoardTraits(gameUnit);
    }

    // Update traits to owner's board
    private static void UpdateBoardTraits(GameUnit gameUnit)
    {
        // Update traits only if there is no same unit on board
        if (!gameUnit.Owner.IsSameUnitOnBoard(gameUnit))
        {
            foreach (Trait trait in gameUnit.Traits)
            {
                List<GameUnit> unitsWithTrait = gameUnit.Owner.GetUnitsWithTrait(trait);
                int unitCount = unitsWithTrait.Count;
                int currentTraitStage = GetBoardTraitStage(trait, unitCount);

                // Get last trait stage according to the unit addition or removal from board
                int lastTraitStage = gameUnit.IsOnBoard ? GetBoardTraitStage(trait, unitCount - 1) : GetBoardTraitStage(trait, unitCount + 1);
                trait.UpdateTrait(unitsWithTrait, gameUnit, currentTraitStage, lastTraitStage);
                int lastUnitCount = gameUnit.IsOnBoard ? unitCount - 1 : unitCount + 1;
                UIManager.Instance.UpdateTraitUI(trait, currentTraitStage, unitCount, lastUnitCount);
            }
        }
    }

    // Get the stage of the trait according to how many units are on board
    public static int GetBoardTraitStage(Trait trait, int unitCount)
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

    public void OnUnitDragStarted()
    {
        foreach (Hex hex in _hexes.Where(hex => hex.X < 4))
        {
            Transform border = hex.transform.Find("Border");
            for (int i = 0; i < border.childCount; i++)
            {
                SpriteRenderer spriteRenderer = border.GetChild(i).GetComponent<SpriteRenderer>();
                spriteRenderer.color = Color.cyan;
            }
        }
    }

    public void OnUnitDragStopped()
    {
        foreach (Hex hex in _hexes.Where(hex => hex.X < 4))
        {
            Transform border = hex.transform.Find("Border");
            for (int i = 0; i < border.childCount; i++)
            {
                SpriteRenderer spriteRenderer = border.GetChild(i).GetComponent<SpriteRenderer>();
                spriteRenderer.color = Color.white;
            }
        }
    }
}
