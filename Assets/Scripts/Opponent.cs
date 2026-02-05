using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Opponent : Player
{
    public static Opponent Instance { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    protected override void OnRoundStateChanged(RoundState newState)
    {
        base.OnRoundStateChanged(newState);
        switch (newState)
        {
            case RoundState.Preparation:
                PrepareEnemyUnits();
                break;
            case RoundState.Battle:
                BattleEnemy();
                break;
        }
        base.OnRoundStateChanged(newState);
    }

    private void BattleEnemy()
    {
        Debug.Log(_bench.BenchUnits.Count);
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

    private void PrepareEnemyUnits()
    {
        CreateEnemyUnits();
    }

    public void CreateEnemyUnits()
    {
        _bench.CreateEnemyUnitByName("Pug", 1);
        _bench.CreateEnemyUnitByName("Dog", 1);
    }
}
