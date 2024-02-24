using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Hp;
    public int Gold;
    public int Xp;
    public int Lvl;
    public string PlayerName { get; protected set; }
    
    [SerializeField] protected Bench _bench;
    protected List<GameUnit> _boardUnits = new();
    public int boardLimit;
    public List<Trait> ActiveTraits = new();

    public Bench Bench { get => _bench; set => _bench = value; }
    public List<GameUnit> BoardUnits { get => _boardUnits; set => _boardUnits = value; }

    private readonly int[] _xpTable = new int[] { 0, 2, 6, 10, 20, 36, 56, 80, 100 };

    private void Start()
    {
        boardLimit = 20;     
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

    // Returns true if player has the gameUnit on bench or board
    public bool HasUnit(GameUnit gameUnit)
    {
        return _boardUnits.Contains(gameUnit) || _bench.BenchUnits.Contains(gameUnit);
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
