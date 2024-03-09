using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TraitInfo : MonoBehaviour
{
    [SerializeField] private GameObject _traitInfo;

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Trait"))
        {
            if (hit.collider.TryGetComponent(out Transform traitTransform))
            {
                ShowTraitInfo(traitTransform);
            }
        }
        else
        {
            HideTraitInfo();
        }
    }


    public void ShowTraitInfo(Transform traitTransform)
    {

        _traitInfo.SetActive(true);
        _traitInfo.transform.position = new Vector3(_traitInfo.transform.position.x, traitTransform.position.y);
        if (traitTransform.TryGetComponent(out TraitData traitData))
        {
            UpdateVisuals(traitData);
        }
    }

    private void UpdateVisuals(TraitData traitData)
    {
        // Trait name
        TextMeshProUGUI traitNameText = _traitInfo.transform.Find("TraitName").GetComponent<TextMeshProUGUI>();
        traitNameText.text = traitData.TraitName;

        // Description
        TextMeshProUGUI descriptionText = _traitInfo.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        descriptionText.text = traitData.Description;

        // Stages description
        Transform stagesTransfrom = _traitInfo.transform.Find("StagesTexts");
        TextMeshProUGUI stage1Text = stagesTransfrom.transform.Find("Stage1").GetComponent<TextMeshProUGUI>();
        if (traitData.Stage1Text != null)
        {
            stage1Text.text = traitData.Stage1Text;
        }
        TextMeshProUGUI stage2Text = stagesTransfrom.transform.Find("Stage2").GetComponent<TextMeshProUGUI>();
        if (traitData.Stage2Text != null)
        {
            stage2Text.text = traitData.Stage2Text;

        }
        TextMeshProUGUI stage3Text = stagesTransfrom.transform.Find("Stage3").GetComponent<TextMeshProUGUI>();
        if (traitData.Stage3Text != null)
        {
            stage3Text.text = traitData.Stage3Text;
        }
        TextMeshProUGUI stage4Text = stagesTransfrom.transform.Find("Stage4").GetComponent<TextMeshProUGUI>();
        if (traitData.Stage4Text != null)
        {
            stage4Text.text = traitData.Stage4Text;
        }

        // Unit images
        // Set all inactive
        Transform unitsTransform = _traitInfo.transform.Find("Units");
        for (int i = 0; i < unitsTransform.childCount; i++)
        {
            GameObject unitGo = unitsTransform.GetChild(i).gameObject;
            unitGo.SetActive(false);
        }
        // Add only units with trait images
        List<GameUnit> sortedUnits = traitData.UnitsWithTrait.OrderBy(unit => unit.UnitData.Rarity).ToList();
        for (int i = 0; i < sortedUnits.Count; i++)
        {
            GameObject unitGo = unitsTransform.GetChild(i).gameObject;
            unitGo.SetActive(true);
            unitGo.transform.Find("Image").GetComponent<Image>().sprite = sortedUnits[i].UnitData.UnitImage;
            Image unitFrame = unitGo.transform.Find("Frame").GetComponent<Image>();
            if (sortedUnits[i].UnitData.Rarity == UnitRarity.Common)
            {
                unitFrame.color = Color.gray;
            }
            if (sortedUnits[i].UnitData.Rarity == UnitRarity.Uncommon)
            {
                unitFrame.color = Color.green;
            }
            if (sortedUnits[i].UnitData.Rarity == UnitRarity.Rare)
            {
                unitFrame.color = Color.red;
            }
            if (sortedUnits[i].UnitData.Rarity == UnitRarity.Epic)
            {
                unitFrame.color = new Color(188f,73f, 164f); 
            }
            if (sortedUnits[i].UnitData.Rarity == UnitRarity.Legendary)
            {
                unitFrame.color = new Color(226f, 231f, 77f);
            }


        }
    }

    public void HideTraitInfo()
    {
        _traitInfo.SetActive(false);

    }
}
