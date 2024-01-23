using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Units Database", menuName = "Game/Units Database")]
public class UnitsDatabase : ScriptableObject
{
    public List<UnitData> Units;

    // Public reference to the instance of UnitsDatabase
    public static UnitsDatabase Instance;

    private void OnEnable()
    {
        // Assign the instance when the scriptable object is loaded
        Instance = this;
    }

    public UnitData FindUnitById(int id)
    {
        return Units.Find(unit => unit.Id == id);
    }

}
