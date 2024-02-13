using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void CreateEnemyUnit()
    {
        //Bench.CreateGameUnit(this,1001,1);
        Bench.CreateGameUnit(this,1002,1);
        foreach (GameUnit gameUnit in Bench.Units)
        {
            gameUnit.PlaceOnHex(Board.Instance.GetHex(5, 1));
            BoardUnits.Add(gameUnit);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateEnemyUnit();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
