using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public int Hp { get; private set; }
    public int Gold { get; private set; }
    public int Xp { get; private set; }
    public int Lvl { get; private set; }
    public string PlayerName { get; private set; }
    public Bench UnitsBench { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
       
        GameObject playerBenchObject = GameObject.Find("PlayerBench");

        // Check if the GameObject was found
        if (playerBenchObject != null)
            UnitsBench = playerBenchObject.GetComponent<Bench>();
        else Debug.LogError("No Object of name PlayerBench found");

        Hp = 100;
        Gold = 10;
        PlayerName = "Spite";
        Lvl = 1;
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
}
