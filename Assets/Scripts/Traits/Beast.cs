using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Trait", menuName = "Game/Traits/Beast")]

// Give attackDamage buff to all units with this trait
public class Beast : Trait
{
    private readonly int[] damageBuff = new int[] {0,5, 10, 20 };

    // Update the trait stage according to the current stage
    public override void UpdateTrait(List<BoardUnit> unitsWithTrait, BoardUnit lastUnit, int currentStage, int lastStage)
    {
        // Update the stage for each unit with this trait on board
        if (currentStage > lastStage)
        {
            StageUp(unitsWithTrait, currentStage);
        }
        else if (currentStage < lastStage)
        {
            StageDown(unitsWithTrait, currentStage);
            DisableTraitForUnit(lastUnit, lastStage); // Since the unit removed is not included in unitsWithTrait
        }
        // Stage hasn't changed but the last unit added/removed also needs to be affacted
        else
        {
            // Unit was added to board
            if (lastUnit.IsOnBoard)
            {
                EnableTraitForUnit(lastUnit, currentStage);
            }
            else
            {
                DisableTraitForUnit(lastUnit, currentStage);
            }
        }

        // Set the current stage of the trait for each unit
        foreach (BoardUnit unitWithTrait in unitsWithTrait)
        {
            unitWithTrait.TraitStages[this] = currentStage;
        }
    }

    // Add buff to units 
    public override void StageUp(List<BoardUnit> unitsWithTrait, int currentStage)
    {
        foreach (BoardUnit unit in unitsWithTrait)
        {
            int totalDamageToAdd = 0;
            int lastUnitStage = unit.TraitStages[this];

            // Sum up the damage according to the stage diff between the unit and the board 
            for (int i = lastUnitStage; i < currentStage; i++)
            {
                totalDamageToAdd += damageBuff[i+1];
            }
            unit.AttackDamage += totalDamageToAdd;
        }
    }

    // Remove buff from units
    public override void StageDown(List<BoardUnit> unitsWithTrait, int currentStage)
    {
        foreach (BoardUnit unit in unitsWithTrait)
        {
            unit.AttackDamage -= damageBuff[currentStage+1];
        }
    }

    // Set the damage buff of the current stage to unit
    private void EnableTraitForUnit(BoardUnit unit, int currentStage)
    {
        int totalDamageToAdd = 0;
        for (int i = 0; i < currentStage; i++)
        {
            totalDamageToAdd += damageBuff[i];
        }
        unit.AttackDamage += totalDamageToAdd;
    }

    // Set the damage buff for the unit to 0
    private void DisableTraitForUnit(BoardUnit unit, int lastStage)
    {
        int totalDamageToRemove = 0;
        for (int i = lastStage; i > 0; i--)
        {
            totalDamageToRemove += damageBuff[i];
        }
        unit.AttackDamage -= totalDamageToRemove;
            Debug.Log(totalDamageToRemove);
        unit.TraitStages[this] = 0;
    }
}
