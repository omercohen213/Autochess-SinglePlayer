using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : Player
{
    public static LocalPlayer Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        Lives = 100;
        Gold = 100;
        PlayerName = "Spite";
        Lvl = 1;
    }
}
