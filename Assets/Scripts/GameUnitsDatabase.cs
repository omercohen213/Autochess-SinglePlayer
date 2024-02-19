using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Units Database", menuName = "Game/Units Database/Game Units Database")]
public class GameUnitsDatabase : UnitsDatabase
{
    // Private field to hold the instance
    private static GameUnitsDatabase _instance;

    // Property to access the instance
    public static GameUnitsDatabase Instance
    {
        // Getter
        get
        {
            if (_instance == null)
            {
                // If the instance hasn't been assigned yet, try to find it in the project assets
                _instance = Resources.Load<GameUnitsDatabase>("GameUnitsDatabase");
            }
            return _instance;
        }

        // Setter
        set
        {
            _instance = value;
        }
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
