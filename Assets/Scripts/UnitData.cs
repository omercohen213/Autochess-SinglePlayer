using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Base Unit Data")]
    //public int Id;
    public string UnitName;
    public int Cost;
    public UnitRarity Rarity;

    [Header("ShopUnit Data")]
    public Sprite ShopImage;

    [Header("GameUnit Data")]
    public GameObject UnitPrefab;
    public int MaxHp;
    public int MaxMp;
    public int BaseAttackDamage;
    public int BaseArmor;
    public int Range;

    [Header("Traits Data")]
    public List<Trait> Traits;
}

