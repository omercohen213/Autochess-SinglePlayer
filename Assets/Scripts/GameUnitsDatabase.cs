using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Units Database", menuName = "Game/Units Database/Game Units Database")]
public class GameUnitsDatabase : UnitsDatabase
{
    public static GameUnitsDatabase Instance;
    private void OnEnable()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
