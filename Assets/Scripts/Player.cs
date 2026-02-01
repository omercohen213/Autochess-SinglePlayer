using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int _lives;
    public int Gold;
    public int Xp;
    public int Lvl;
    public string PlayerName { get; protected set; }
    
    [SerializeField] protected Bench _bench;
    [SerializeField] protected List<GameUnit> _boardUnits = new();
    public int boardLimit;
    public List<Trait> ActiveTraits = new();

    public Bench Bench { get => _bench; set => _bench = value; }
    public List<GameUnit> BoardUnits { get => _boardUnits; set => _boardUnits = value; }
    public int Lives { get => _lives; set => _lives = value; }

    private readonly int[] _xpTable = new int[] { 0, 2, 6, 10, 20, 36, 56, 80, 100 };
    private RoundManager _roundManager;

    protected virtual void Awake()
    {
    }
    
    private void OnEnable()
    {
        _roundManager = RoundManager.Instance;
        if (_roundManager != null)
        {
            _roundManager.OnPhaseChanged += OnPhaseChanged;
        }
    }

    private void OnDisable()
    {
        if (_roundManager != null)
        {
            _roundManager.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    private void Start()
    {
        boardLimit = 10;
        _lives = 3;
    }

    protected virtual void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                break;
            case GamePhase.RoundStart:
                ShowBoardUnitsBars();
                break;
            case GamePhase.RoundOver:
                break;
        }
    }

    // Show hp and mp bar of units on board
    private void ShowBoardUnitsBars()
    {
        foreach (GameUnit gameUnit in _boardUnits)
        {
            gameUnit.ShowBars();
        }
    }

    // Decrease gold by amount
    public void PayGold(int amount)
    {
        Gold -= amount;
        UIManager.Instance.UpdateGoldUI();
    }


    // Add gold by amount
    public void GainGold(int amount)
    {
        Gold += amount;
        //UIManager.Instance.UpdateGoldUI();
    }

    // Add amount to player xp
    public void GainXp(int amount)
    {
        Xp += amount;
        UIManager.Instance.UpdateXpUI();
        CheckLevelUp();
    }

    // Check if player leveled up through the xp gain
    private void CheckLevelUp()
    {
        int xpToLevelUp = GetXpToLevelUp(Lvl);
        if (Xp >= xpToLevelUp)
        {
            Xp = 0;
            OnLevelUp();
        }
    }

    // Gain the level up benefits
    private void OnLevelUp()
    {
        Lvl++;
        boardLimit++;
        UIManager.Instance.UpdateBoardLimit();
        UIManager.Instance.UpdatePlayerLvlUI();
    }

    public List<GameUnit> GetUnitsWithTrait(Trait trait)
    {
        List<GameUnit> unitsWithTrait = new();
        foreach (GameUnit unit in _boardUnits)
        {
            if (unit.Traits.Contains(trait) && !unitsWithTrait.Any(u => u.Equals(unit)))
            {
                unitsWithTrait.Add(unit);
            }
        }
        return unitsWithTrait;
    }


    // Check if there is the same unit already on board 
    public bool IsSameUnitOnBoard(GameUnit gameUnit)
    {
        if (_boardUnits.Contains(gameUnit))
        {
            List<GameUnit> temp = new(_boardUnits);
            temp.Remove(gameUnit);

            foreach (GameUnit playerUnit in temp)
            {
                if (playerUnit.Equals(gameUnit) && playerUnit.IsOnBoard)
                {
                    return true;
                }
            }
        }     
        return false;
    }

    // Returns true if player has any gameUnit on board
    public bool HasAnyUnitOnBoard()
    {
        return _boardUnits.Any();
    }

    // Returns true if player has the gameUnit of name of on bench or board
    public bool HasUnit(string unitName)
    {
        foreach (GameUnit gameUnit in _boardUnits)
        {
            if (gameUnit.UnitName == unitName)
            {
                return true;
            }
        }
        foreach (GameUnit gameUnit in _bench.BenchUnits)
        {
            if (gameUnit.UnitName == unitName)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBoardLimitReached()
    {
        return _boardUnits.Count >= boardLimit; 
    }

    public int GetXpToLevelUp(int lvl)
    {
        return _xpTable[lvl];
    }
}
