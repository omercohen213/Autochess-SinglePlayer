using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Opponent : Player
{
    public static Opponent Instance { get; private set; }


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
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                PrepareEnemy();
                break;
            case GamePhase.Battle:
                BattleEnemy();
                break;
            case GamePhase.BattleWon:
            case GamePhase.BattleLost:
                break;
        }
    }

    private void BattleEnemy()
    {
        foreach (GameUnit gameUnit in _bench.BenchUnits)
        {
            if (gameUnit.UnitName == "Pug")
            {
                gameUnit.PlaceOnHex(Board.Instance.GetHex(5, 1));
            }
           /* if (gameUnit.UnitName == "Dog")
            {
                gameUnit.PlaceOnHex(Board.Instance.GetHex(6, 3));

            }*/
            //Hex randomHex = GetRandomHex();
            //gameUnit.PlaceOnHex(randomHex);
            _boardUnits.Add(gameUnit);
        }
    }

    private Hex GetRandomHex()
    {
        int x = Random.Range(4, 8);
        int y = Random.Range(0, 5);
        return Board.Instance.GetHex(x, y);
    }

    private void PrepareEnemy()
    {
        CreateEnemyUnits();
    }

    public void CreateEnemyUnits()
    {
        _bench.CreateGameUnitByName(this, "Pug", 1);
        _bench.CreateGameUnitByName(this, "Dog", 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
