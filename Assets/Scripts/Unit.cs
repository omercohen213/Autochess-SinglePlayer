using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Cost { get; private set; }
    public List<Trait> Traits { get; private set; }

    [SerializeField] private string unitName;
    public string UnitName { get => unitName; private set => unitName = value; }

    [SerializeField] private UnitData unitData;
    public UnitData UnitData { get => unitData; private set => unitData = value; }

    private void Awake()
    {
        Traits = new();
    }

    public virtual void SetUnitData(int id)
    {
        UnitData = UnitsDatabase.Instance.FindUnitById(id);
        Traits = UnitData.Traits;
        Cost = UnitData.Cost;
        UnitName = UnitData.UnitName;
    }
}
