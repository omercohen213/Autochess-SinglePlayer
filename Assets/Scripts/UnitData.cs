using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{

    [Header("Base Unit Data")]
    public int Id;
    public string UnitName;
    public int Cost;

    [Header("ShopUnit Data")]
    public Image ShopImage;

    [Header("BoardUnit Data")]
    public int MaxHp;
    public int MaxMp;
    public int BaseAttackDamage;
    public int BaseArmor;
    public Sprite Sprite; 

    [Header("Traits Data")]
    public List<Trait> Traits;

    [Header("Ability Data")]
    public string AbilityName;
    // Add any other ability-related properties
}
