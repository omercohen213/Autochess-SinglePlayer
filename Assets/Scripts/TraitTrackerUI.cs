using Assets.HeroEditor.Common.Scripts.CharacterScripts.Firearms.Enums;
using Assets.HeroEditor.Common.Scripts.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TraitTrackerUI : MonoBehaviour
{
    [SerializeField] private Transform _activeTraitsTransform;
    [SerializeField] private Transform _inactiveTraitsTransform;

    public void UpdateTraits(Trait trait, int currentStage, int unitCount, int lastUnitCount)
    {
        // Unit was added to board with trait for the first time 
        if (unitCount == 1 && lastUnitCount == 0)
        {
            // In case activation needs only one unit
            if (IsTraitActive(trait, unitCount))
            {
                AddTrait(trait, true, currentStage, unitCount);
                _activeTraitsTransform.SetActive(true);
            }
            // Trait is added inactive
            else
            {
                AddTrait(trait, false, currentStage, unitCount);
            }
        }

        else if (unitCount >= 1)
        {
            // A unit was added
            if (unitCount > lastUnitCount)
            {
                // Trait is getting activated
                if (currentStage >= 1)
                {
                    RemoveTrait(trait);
                    AddTrait(trait, true, currentStage, unitCount);
                }
            }

            // A unit was removed
            else
            {
                RemoveTrait(trait);

                // Check if trait is now inactive
                if (currentStage == 0)
                {
                    AddTrait(trait, false, currentStage, unitCount);
                }
                else
                {
                    AddTrait(trait, true, currentStage, unitCount);
                }
            }
        }

        // A unit was removed and there are no more units with this trait on board
        else if (unitCount == 0 && lastUnitCount == 1)
        {
            RemoveTrait(trait);

            // If there are no active traits,
            // disable _activeTraitsTransform to allow the inactive traits to be on top
            foreach (Transform child in _activeTraitsTransform)
            {
                if (child.gameObject.activeSelf)
                {
                    break;
                }
                _activeTraitsTransform.SetActive(false);
            }
        }
    }

    private bool IsTraitActive(Trait trait, int unitCount)
    {
        return unitCount >= trait.UnitNumNeeded[0];
    }

    // Create the trait transform and set initial texts and images
    public void AddTrait(Trait trait, bool isActive, int currentStage, int unitCount)
    {
        Transform traitTransform = null;
        Transform parent;
        if (isActive)
        {
            parent = _activeTraitsTransform;
        }
        else
        {
            parent = _inactiveTraitsTransform;
        }

        foreach (Transform child in parent)
        {
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                traitTransform = child;
                break;
            }
        }
        UpdateVisuals(trait, traitTransform, currentStage, unitCount);
    }


    private void UpdateVisuals(Trait trait, Transform traitTransform, int currentStage, int unitCount)
    {
        if (traitTransform != null)
        {
            traitTransform.gameObject.name = trait.TraitName;

            // Set trait name
            Transform nameTransform = traitTransform.Find("Name");
            if (nameTransform.TryGetComponent<TextMeshProUGUI>(out var nameText))
            {
                nameText.text = trait.TraitName;
            }

            // Set trait image
            Transform imageTransform = traitTransform.Find("Icon");
            if (imageTransform.TryGetComponent<Image>(out var image))
            {
                image.sprite = trait.TraitSprite;
            }

            Transform currentStageTransform = traitTransform.Find("CurrentStage").Find("Text");
            if (currentStageTransform.TryGetComponent<TextMeshProUGUI>(out var currentStageText))
            {
                currentStageText.text = currentStage.ToString();
            }

            Transform stageProgressTransform = traitTransform.Find("StageProgress").Find("Text");
            if (stageProgressTransform.TryGetComponent<TextMeshProUGUI>(out var stageProgressText))
            {
                stageProgressText.text = unitCount.ToString() + "/" + trait.UnitNumNeeded[currentStage];
            }
        }
        else
        {
            Debug.LogWarning("No Trait Transform");
        }
    }

    // Find the trait and set the gameobject inactive (objectPooling?)
    private void RemoveTrait(Trait trait)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform traitTransfrom = transform.GetChild(i).Find(trait.TraitName);
            if (traitTransfrom != null)
            {
                traitTransfrom.gameObject.SetActive(false);
            }
        }
    }
}