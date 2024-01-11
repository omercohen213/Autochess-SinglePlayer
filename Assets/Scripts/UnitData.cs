using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public int cost;
    public int hp;
    public int mp;
    public int attackDamage;
    public int armor;

    [Header("Ability Data")]
    public string abilityName;
    // Add any other ability-related properties
}
