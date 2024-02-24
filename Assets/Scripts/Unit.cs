using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    protected UnitData _unitData;
    protected string _unitName;
    protected int _cost;
    protected UnitRarity _rarity;
    [SerializeField] protected List<Trait> _traits;
    public string UnitName { get => _unitName; private set => _unitName = value; }
    public UnitData UnitData { get => _unitData; private set => _unitData = value; }
    public List<Trait> Traits { get => _traits; set => _traits = value; }
    public int Cost { get => _cost; set => _cost = value; }

    private void Awake()
    {
        _traits = new();
    }

    public virtual void SetUnitData(UnitData unitData)
    {
        _unitData = unitData;
        _cost = _unitData.Cost;
        _unitName = _unitData.UnitName;
        _rarity = _unitData.Rarity;
        _traits = _unitData.Traits;
    }
}
