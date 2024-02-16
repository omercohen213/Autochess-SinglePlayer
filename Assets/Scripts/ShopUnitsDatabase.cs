using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Units Database", menuName = "Game/Units Database/Shop Units Database")]
public class ShopUnitsDatabase : UnitsDatabase
{
    public static ShopUnitsDatabase Instance;

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
