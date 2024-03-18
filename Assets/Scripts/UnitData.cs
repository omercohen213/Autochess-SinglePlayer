using HeroEditor.Common.Enums;
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
    public string UnitName;
    public int Cost;
    public UnitRarity Rarity;
    public Sprite UnitImage;

    [Header("GameUnit Data")]
    public GameObject UnitPrefab;
    public int MaxHp;
    public int MaxMp;
    public int BaseAttackDamage;
    public int BaseAbilityPower;
    public float BaseAttackSpeed;
    public int BaseArmor;
    public int BaseMagicResist;
    public int Range;
    public Weapon Weapon;
    public GameObject WeaponProjectile;

    [Header("Traits Data")]
    public List<Trait> Traits;
}

