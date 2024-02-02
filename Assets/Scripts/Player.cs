using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public int Hp;
    public int Gold;
    public int Xp;
    public int Lvl;
    public string PlayerName { get; private set; }

    [SerializeField] public Bench Bench;
    public List<BoardUnit> BoardUnits;
    public List<Trait> ActiveTraits; 


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        Hp = 100;
        Gold = 10;
        PlayerName = "Spite";
        Lvl = 1;

        ActiveTraits= new();
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
        int _xpToLevelUp = GameManager.Instance.GetXpToLevelUp(Lvl);
        if (Xp > _xpToLevelUp)
        {
            Xp = 0;
            OnLevelUp();
        }
    }

    // Gain the level up benefits
    private void OnLevelUp()
    {
        UIManager.Instance.UpdatePlayerLvlUI();
    }

    public List<BoardUnit> GetUnitsWithTrait(Trait trait)
    {
        List<BoardUnit> unitsWithTrait = new ();
        foreach (BoardUnit unit in BoardUnits)
        {
            if (unit.Traits.Contains(trait))
            {
                unitsWithTrait.Add(unit);
            }
        }
        return unitsWithTrait;
    }
}
