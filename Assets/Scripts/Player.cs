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

    public Bench Bench;
    public List<GameUnit> BoardUnits = new();
    public int boardLimit;
    public List<Trait> ActiveTraits = new();

    private void Start()
    {
        boardLimit = 2;     
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
        UIManager.Instance.UpdateGoldUI();
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
        int xpToLevelUp = GameManager.Instance.GetXpToLevelUp(Lvl);
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
        UIManager.Instance.UpdateXpUI();
        UIManager.Instance.UpdateBoardLimit();
        UIManager.Instance.UpdatePlayerLvlUI();
    }

    public List<GameUnit> GetUnitsWithTrait(Trait trait)
    {
        List<GameUnit> unitsWithTrait = new();
        foreach (GameUnit unit in BoardUnits)
        {
            if (unit.Traits.Contains(trait) && !unitsWithTrait.Any(u => u.Equals(unit)))
            {
                unitsWithTrait.Add(unit);
            }
        }
        return unitsWithTrait;
    }


    // Check if there is the same unit already on board
    public bool IsSameUnitOnBoard(GameUnit unit)
    {
        List<GameUnit> temp = new(BoardUnits);
        temp.Remove(unit);

        foreach (GameUnit playerUnit in temp)
        {
            if (playerUnit.Equals(unit) && playerUnit.IsOnBoard)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBoardLimitReached()
    {
        return BoardUnits.Count >= boardLimit; 
    }
}
