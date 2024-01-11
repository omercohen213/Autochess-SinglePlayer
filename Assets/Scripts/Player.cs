using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public int Hp { get; private set; }
    public int Gold { get; private set; }
    public int Xp { get; private set; }
    public string PlayerName { get; private set; }

    public UnitsBench UnitsBench { get; private set; }


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
            UnitsBench = playerBenchObject.GetComponent<UnitsBench>();
        else Debug.LogError("No Object of name PlayerBench found");
        
    }
    private void Start()
    {
        Hp = 100;
        Gold = 10;
        PlayerName = "Spite";
    }

    // Decrease gold by amount
    public void PayGold(int amount)
    {
        Gold -= amount;
    }

    // Add amount to player xp
    public void GainXp(int amount)
    {
        Xp += amount;
    }


}
