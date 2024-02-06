using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : Player
{
    public static Player Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        Hp = 100;
        Gold = 100;
        PlayerName = "Spite";
        Lvl = 1;

        ActiveTraits = new();
    }
}
