using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitInfo : MonoBehaviour
{
    private GameObject _unitInfo;
    private RaycastHit2D[] _hits;
    private int _shopLayer;
    private int _benchLayer;
    private int _boardLayer;
    private int _layerMask;

    private void Awake()
    {
        _unitInfo = FindInActiveObjectByName("UnitInfo");
    }



    private void Start()
    {
        _boardLayer = LayerMask.NameToLayer("Board");
        _shopLayer = LayerMask.NameToLayer("Shop");
        _benchLayer = LayerMask.NameToLayer("Bench");
        _layerMask = ~(1 << _shopLayer) & ~(1 << _benchLayer) & ~(1 << _boardLayer);
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity, _layerMask);

        foreach (RaycastHit2D hit in _hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("GameUnit"))
            {
                Debug.Log(hit.collider);
                if (hit.collider.TryGetComponent(out GameUnit gameUnit))
                {
                    //ShowUnitInfo(gameUnit);
                }
                else
                {
                    HideUnitInfo();
                }
            }
           
        }
    }

    public void ShowUnitInfo(GameUnit gameUnit)
    {
        _unitInfo.SetActive(true);
        _unitInfo.transform.position = new Vector3(gameUnit.transform.position.x, gameUnit.transform.position.y - 1.5f);
        UpdateVisuals(gameUnit);
    }

    public void HideUnitInfo()
    {
        Debug.Log("s");
        _unitInfo.SetActive(false);
    }

    private void UpdateVisuals(GameUnit gameUnit)
    {
        Transform statsTransform = _unitInfo.transform.Find("Stats");

        // Texts
        TextMeshProUGUI AttackDamageText = statsTransform.Find("AttackDamage").Find("Text").GetComponent<TextMeshProUGUI>();
        AttackDamageText.text = gameUnit.AttackDamage.ToString();

        TextMeshProUGUI AbilityPowerText = statsTransform.Find("AbilityPower").Find("Text").GetComponent<TextMeshProUGUI>();
        AbilityPowerText.text = gameUnit.AbilityPower.ToString();

        TextMeshProUGUI ArmorText = statsTransform.Find("Armor").Find("Text").GetComponent<TextMeshProUGUI>();
        ArmorText.text = gameUnit.Armor.ToString();

        TextMeshProUGUI MagicResistText = statsTransform.Find("MagicResist").Find("Text").GetComponent<TextMeshProUGUI>();
        MagicResistText.text = gameUnit.MagicResist.ToString();

        TextMeshProUGUI RangeText = statsTransform.Find("Range").Find("Text").GetComponent<TextMeshProUGUI>();
        RangeText.text = gameUnit.Range.ToString();

        TextMeshProUGUI AttackSpeedText = statsTransform.Find("AttackSpeed").Find("Text").GetComponent<TextMeshProUGUI>();
        AttackSpeedText.text = gameUnit.AttackSpeed.ToString();

        TextMeshProUGUI CritChanceText = statsTransform.Find("CritChance").Find("Text").GetComponent<TextMeshProUGUI>();
        CritChanceText.text = (gameUnit.CritChance * 100).ToString() + "%";

        TextMeshProUGUI CritDamageText= statsTransform.Find("CritDamage").Find("Text").GetComponent<TextMeshProUGUI>();
        CritDamageText.text = (gameUnit.CritDamage *100).ToString() + "%";
    }

    GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].name == name)
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }
}
