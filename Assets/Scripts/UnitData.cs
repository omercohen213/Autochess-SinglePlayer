using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    public int Id;
    public string UnitName;
    public int Cost;
    public int Hp;
    public int Mp;
    public int AttackDamage;
    public int Armor;
    public Sprite UnitSprite;
    public Image UnitShopImage;

    [Header("Ability Data")]
    public string AbilityName;
    // Add any other ability-related properties
}
