using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Cost { get; private set; }

    [SerializeField] private string unitName;
    public string UnitName { get => unitName; private set => unitName = value; }

    [SerializeField] private UnitData unitData;
    public UnitData UnitData { get => unitData; private set => unitData = value; }

    //public Trait[] traits; 
    public virtual void SetUnitData(int id)
    {
        UnitData = UnitsDatabase.Instance.FindUnitById(id);
        Cost = UnitData.Cost;
        UnitName = UnitData.UnitName;
    }
}
