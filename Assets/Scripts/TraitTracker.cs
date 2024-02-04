using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TraitTracker : MonoBehaviour
{
    [SerializeField] private Transform _activeTraitsTransform;
    [SerializeField] private Transform _inactiveTraitsTransform;
    [SerializeField] private GameObject _traitPrefab;
    [SerializeField] private GameObject _stageTextPrefab;

    public void UpdateTraits(Trait trait, int currentStage, int unitCount, int lastUnitCount)
    {
        // Unit was added to board with trait for the first time 
        if (unitCount == 1 && lastUnitCount == 0)
        {
            // Trait is added inactive
            AddTrait(trait);
            UpdateUnitCount(trait, unitCount, currentStage);

            // In case activation needs only one unit
            if (IsTraitActive(trait, unitCount))
            {
                SetTraitUIActive(trait);
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
                    Transform activeTraitTransform = _activeTraitsTransform.Find(trait.name);
                    if (IsTraitActive(trait, unitCount) && activeTraitTransform == null)
                    {
                        SetTraitUIActive(trait);
                    }
                }
            }

            // A unit was removed
            else
            {
                if (currentStage == 0)
                {
                    SetTraitUIInactive(trait);
                }
            }

            UpdateUnitCount(trait, unitCount, currentStage);
        }

        // A unit was removed and there are no more units with this trait on board
        else if (unitCount == 0 && lastUnitCount == 1)
        {
            RemoveTrait(trait);
        }
    }

    private bool IsTraitActive(Trait trait, int unitCount)
    {
        return unitCount >= trait.UnitNumNeeded[0];
    }

    // Create the trait transform and set initial texts and images
    public void AddTrait(Trait trait)
    {
        GameObject traitGo = Instantiate(_traitPrefab, _inactiveTraitsTransform);
        traitGo.name = trait.name;

        // Set trait name
        Transform nameTransform = traitGo.transform.Find("Name");
        if (nameTransform.TryGetComponent<TextMeshProUGUI>(out var nameText))
        {
            nameText.text = trait.traitName;
        }

        // Set trait image
        Transform imageTransform = traitGo.transform.Find("Image");
        if (imageTransform.TryGetComponent<Image>(out var image))
        {
            image.sprite = trait.traitSprite;
        }

        SetStagesBackgroundWidth(trait, traitGo.transform);
        SetStagesText(trait, traitGo.transform);
    }

    // Set parent to activeTraits and color alpha to 1 
    private void SetTraitUIActive(Trait trait)
    {
        Transform traitTransform = _inactiveTraitsTransform.Find(trait.name);
        if (traitTransform != null)
        {
            traitTransform.SetParent(_activeTraitsTransform);
            SetColorAlpha(traitTransform, 1f);
        }
        else Debug.LogWarning("No object of name " + trait.name);
    }

    // Set parent to inactiveTraits and lower color alpha
    private void SetTraitUIInactive(Trait trait)
    {
        Transform traitTransform = _activeTraitsTransform.Find(trait.name);
        if (traitTransform != null)
        {
            traitTransform.transform.SetParent(_inactiveTraitsTransform);
            SetColorAlpha(traitTransform, 0.75f);
        }
        else Debug.Log("No object of name " + trait.name);
    }

    // Find the trait and set the gameobject inactive (objectPooling?)
    private void RemoveTrait(Trait trait)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform traitTransfrom = transform.GetChild(i).Find(trait.name);
            if (traitTransfrom != null)
            {
                traitTransfrom.gameObject.SetActive(false);
                Destroy(traitTransfrom.gameObject);
            }
        }
    }

    // Update trait unit count and highlight the current stage
    public void UpdateUnitCount(Trait trait, int unitCount, int currentStage)
    {
        Transform traitTransform = _inactiveTraitsTransform.Find(trait.name);
        if (traitTransform == null)
        {
            Debug.Log("active");
            traitTransform = _activeTraitsTransform.Find(trait.name);
        }
        Debug.Log("inactive");

        if (traitTransform != null)
        {
            // Set unit count text
            Transform unitCountTransform = traitTransform.Find("UnitCount");
            if (unitCountTransform != null)
            {
                if (unitCountTransform.TryGetComponent<TextMeshProUGUI>(out var unitCountText))
                {
                    unitCountText.text = unitCount.ToString();
                    Debug.Log("text "+ unitCountText.text + " unitcount " + unitCount.ToString());
                }
                else
                {
                    Debug.LogWarning("Trait " + trait.name + " has no unit count text component");
                }
            }

            // Highlight current stage
            if (currentStage >= 1)
            {
                Transform stagesTransform = traitTransform.Find("Stages");
                if (stagesTransform != null)
                {
                    TextMeshProUGUI[] stageTexts = stagesTransform.GetComponentsInChildren<TextMeshProUGUI>();
                    for (int i = 0; i < stageTexts.Length; i++)
                    {
                        if (stageTexts[i].text == trait.UnitNumNeeded[currentStage - 1].ToString())
                        {
                            stageTexts[i].color = Color.yellow;
                        }
                        else
                        {
                            stageTexts[i].color = Color.white;
                        }
                    }
                }
            }
        }

        else
        {
            Debug.LogWarning("Trait " + trait.name + " not found");
        }


    }

    // Set the alpha color of the image to 1
    private void SetColorAlpha(Transform traitTransform, float alpha)
    {
        Debug.Log(traitTransform.gameObject.name);
        Color color;

        // Image

        if (traitTransform.Find("Image").TryGetComponent<Image>(out var image))
        {
            color = image.color;
            image.color = new Color(color.r, color.g, color.b, alpha);
        }
        else
        {
            Debug.Log("Missing trait image object");
        }

        // Background Image

        if (traitTransform.Find("ImageBackground").TryGetComponent<Image>(out var backgroundImage))
        {
            color = backgroundImage.color;
            backgroundImage.color = new Color(color.r, color.g, color.b, alpha);
        }
        else
        {
            Debug.Log("Missing trait background image object");
        }
    }

    // Set stages background width to suit stages array length
    private void SetStagesBackgroundWidth(Trait trait, Transform traitTransform)
    {
        if (traitTransform.Find("StagesBackground").TryGetComponent<RectTransform>(out var stagesBackground))
        {
            stagesBackground.sizeDelta = trait.UnitNumNeeded.Length switch
            {
                1 => new Vector2(120, 50),
                2 => new Vector2(150, 50),
                3 => new Vector2(180, 50),
                4 => new Vector2(210, 50),
                _ => new Vector2(180, 50),
            };
        }
    }


    // Set trait stages text in form '{stage} > {stage}'
    private void SetStagesText(Trait trait, Transform traitTransform)
    {
        if (traitTransform.Find("Stages").TryGetComponent<RectTransform>(out var stages))
        {
            for (int i = 0; i < trait.UnitNumNeeded.Length; i++)
            {
                GameObject stageTextGo = Instantiate(_stageTextPrefab, stages);
                if (stageTextGo.TryGetComponent<TextMeshProUGUI>(out var stageTextMesh))
                {
                    stageTextMesh.text = trait.UnitNumNeeded[i].ToString();
                }
                if (i != trait.UnitNumNeeded.Length - 1)
                {
                    GameObject seperatorTextGo = Instantiate(_stageTextPrefab, stages);
                    if (seperatorTextGo.TryGetComponent<TextMeshProUGUI>(out var seperatorTextMesh))
                    {
                        seperatorTextMesh.text = ">";
                    }
                }
            }
        }
    }
}