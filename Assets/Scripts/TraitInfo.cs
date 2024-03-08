using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TraitInfo : MonoBehaviour
{
    [SerializeField] private GameObject _traitInfo;
    [SerializeField] private string _traitName;
    [SerializeField] private string _description;
    [SerializeField] private string _stage1Text;
    [SerializeField] private string _stage2Text;
    [SerializeField] private string _stage3Text;
    [SerializeField] private string _stage4Text;
    [SerializeField] private List<GameUnit> _unitsWithTrait;

    public string TraitName { get => _traitName; set => _traitName = value; }
    public string TraitDescription { get => _description; set => _description = value; }
    public List<GameUnit> UnitsWithTrait { get => _unitsWithTrait; set => _unitsWithTrait = value; }
    public string Stage1Text { get => _stage1Text; set => _stage1Text = value; }
    public string Stage2Text { get => _stage2Text; set => _stage2Text = value; }
    public string Stage3Text { get => _stage3Text; set => _stage3Text = value; }
    public string Stage4Text { get => _stage4Text; set => _stage4Text = value; }

    private void Awake()
    {
        _unitsWithTrait = new();
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Trait"))
        {
            ShowTraitInfo();
        }
    }


public void ShowTraitInfo()
{
    _traitInfo.SetActive(true);
    _traitInfo.transform.position = new Vector3(_traitInfo.transform.position.x, transform.position.y);
    UpdateVisuals();
}

private void UpdateVisuals()
{
    // Trait name
    TextMeshProUGUI traitNameText = _traitInfo.transform.Find("TraitName").GetComponent<TextMeshProUGUI>();
    traitNameText.text = _traitName;

    // Description
    TextMeshProUGUI descriptionText = _traitInfo.transform.Find("Description").GetComponent<TextMeshProUGUI>();
    descriptionText.text = _description;

    // Stages description
    Transform stagesTransfrom = _traitInfo.transform.Find("StagesTexts");
    TextMeshProUGUI stage1Text = stagesTransfrom.transform.Find("Stage1").GetComponent<TextMeshProUGUI>();
    if (_stage1Text != null)
    {
        stage1Text.text = _stage1Text;
    }
    TextMeshProUGUI stage2Text = stagesTransfrom.transform.Find("Stage2").GetComponent<TextMeshProUGUI>();
    if (_stage2Text != null)
    {
        stage2Text.text = _stage2Text;

    }
    TextMeshProUGUI stage3Text = stagesTransfrom.transform.Find("Stage3").GetComponent<TextMeshProUGUI>();
    if (_stage3Text != null)
    {
        stage3Text.text = _stage3Text;
    }
    TextMeshProUGUI stage4Text = stagesTransfrom.transform.Find("Stage4").GetComponent<TextMeshProUGUI>();
    if (_stage4Text != null)
    {
        stage4Text.text = _stage4Text;
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
    for (int i = 0; i < _unitsWithTrait.Count; i++)
    {
        Debug.Log("ss");
        GameObject unitGo = unitsTransform.GetChild(i).gameObject;
        unitGo.SetActive(true);
        unitGo.transform.Find("Image").GetComponent<Image>().sprite = _unitsWithTrait[i].UnitImage;
    }
}

public void HideTraitInfo()
{
    _traitInfo.SetActive(false);

}
}
