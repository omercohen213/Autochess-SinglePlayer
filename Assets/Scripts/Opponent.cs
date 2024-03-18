using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Opponent : Player
{
    public static Opponent Instance { get; private set; }

    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    protected override void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                PrepareEnemy();
                break;
            case GamePhase.Battle:
                BattleEnemy();
                break;
        }
        base.OnPhaseChanged(newPhase);
    }

    private void BattleEnemy()
    {
        foreach (GameUnit gameUnit in _bench.BenchUnits)
        {
            if (gameUnit.UnitName == "Pug")
            {
                Hex randomHex = GetRandomHex();
                gameUnit.PlaceOnHex(randomHex);
                _boardUnits.Add(gameUnit);
            }
            if (gameUnit.UnitName == "Dog")
            {
                Hex randomHex = GetRandomHex();
                gameUnit.PlaceOnHex(randomHex);
                _boardUnits.Add(gameUnit);
            }
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
        _bench.CreateEnemyUnitByName("Pug", 1);
        _bench.CreateEnemyUnitByName("Dog", 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
