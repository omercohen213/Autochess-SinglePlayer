using Assets.HeroEditor.Common.Scripts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    [SerializeField] private Transform _shopUnitsTransform;
    [SerializeField] private List<UnitData> _shopUnitsDatabase = new();

    private List<int[]> _shopUnitsProbabilities;
    private readonly int[] LVL1_PROBABILITIES = new[] { 70, 30, 0, 0, 0 };
    private readonly int[] LVL2_PROBABILITIES = new[] { 55, 30, 15, 0, 0 };
    private readonly int[] LVL3_PROBABILITIES = new[] { 45, 33, 20, 2, 0 };
    private readonly int[] LVL4_PROBABILITIES = new[] { 30, 40, 25, 5, 0 };
    private readonly int[] LVL5_PROBABILITIES = new[] { 19, 30, 35, 10, 1 };

    private readonly int REROLL_COST = 1;
    private readonly int SHOP_UNITS = 5;
    private readonly int XP_COST = 4;
    private Player _player;

    private static Shop _instance;
    public static Shop Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Shop>();
            }
            return _instance;
        }
    }

    [SerializeField] private GameObject _unitSellField;
    public GameObject UnitSellField { get => _unitSellField; set => _unitSellField = value; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        _shopUnitsProbabilities = new() { LVL1_PROBABILITIES, LVL2_PROBABILITIES, LVL3_PROBABILITIES, LVL4_PROBABILITIES, LVL5_PROBABILITIES };

        // Create a copy of the database to override
        foreach (UnitData unitData in ShopUnitsDatabase.Instance.UnitDatas)
        {
            _shopUnitsDatabase.Add(unitData);
        }
        
    }

    private void Start()
    {
        _player = LocalPlayer.Instance;

        if (_unitSellField != null)
        {
            _unitSellField.SetActive(false);
        }

        RerollUnits();
    }

    // Buy reroll on button click
    public void OnRerollClick()
    {
        if (CanAfford(REROLL_COST))
        {
            _player.PayGold(REROLL_COST);
            RerollUnits();
        }
    }

    // Buy xp on button click
    public void OnBuyXpClick()
    {
        if (CanAfford(XP_COST))
        {
            _player.PayGold(XP_COST);
            _player.GainXp(4);
        }
    }

    // Check if player can afford paying goldToPay
    private bool CanAfford(int goldToPay)
    {
        if (_player.Gold >= goldToPay)
            return true;
        else
        {
            Debug.Log("Cannot afford!");
            Debug.Log("Player gold: " + _player.Gold + " Unit cost: " + goldToPay);
            return false;
        }
    }

    // Show new units in shop, randomly chosen from the units database 
    private void RerollUnits()
    {
        // Clear current shop units
        for (int i = 0; i < SHOP_UNITS; i++)
        {
            // Clear shop
            Transform shopUnitParent = _shopUnitsTransform.GetChild(i);
            if (shopUnitParent != null)
            {
                foreach (ShopUnit shopUnit in shopUnitParent.GetComponentsInChildren<ShopUnit>())
                {
                    shopUnit.gameObject.SetActive(false);
                }
            }
        }
        DisableHighlights();

        for (int i = 0; i < SHOP_UNITS; i++)
        {
            // Create the shop unit gameObject
            UnitData unitData = GetRandomUnitData();
            ShopUnit shopUnit = null;
            switch (unitData.Rarity)
            {
                case UnitRarity.Common:
                    shopUnit = _shopUnitsTransform.GetChild(i).Find("ShopUnitCommon").GetComponent<ShopUnit>();
                    break;
                case UnitRarity.Uncommon:
                    shopUnit = _shopUnitsTransform.GetChild(i).Find("ShopUnitUncommon").GetComponent<ShopUnit>();
                    break;
                case UnitRarity.Rare:
                    shopUnit = _shopUnitsTransform.GetChild(i).Find("ShopUnitRare").GetComponent<ShopUnit>();
                    break;
                case UnitRarity.Epic:
                    shopUnit = _shopUnitsTransform.GetChild(i).Find("ShopUnitEpic").GetComponent<ShopUnit>();
                    break;
                case UnitRarity.Legendary:
                    shopUnit = _shopUnitsTransform.GetChild(i).Find("ShopUnitLegendery").GetComponent<ShopUnit>();
                    break;
            }

            if (unitData != null)
            {
                shopUnit.SetUnitData(unitData);
                ShowShopUnit(shopUnit);
                HighlightSameUnits(shopUnit);
            }

            // Add button function
            Button buyButton = shopUnit.transform.parent.gameObject.GetComponent<Button>();
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => BuyUnit(shopUnit));
            buyButton.interactable = true;
        }
    }

    // Update the visual representation
    private void ShowShopUnit(ShopUnit shopUnit)
    {
        // Name
        TextMeshProUGUI shopUnitName = shopUnit.transform.Find("UnitName").GetComponent<TextMeshProUGUI>();
        shopUnitName.text = shopUnit.UnitName;

        // Cost
        TextMeshProUGUI shopUnitCost = shopUnit.transform.Find("Cost").GetComponent<TextMeshProUGUI>();
        shopUnitCost.text = shopUnit.Cost.ToString();

        // Shop unit image
        Image shopUnitImage = shopUnit.transform.Find("UnitImage").GetComponent<Image>();
        shopUnitImage.sprite = shopUnit.ShopImage;

        // Traits
        Transform traitsParent = shopUnit.transform.Find("Traits");
        if (traitsParent != null)
        {
            // Clear traits
            for (int i = 0; i < traitsParent.childCount; i++)
            {
                traitsParent.GetChild(i).SetActive(false);
            }

            // Set icon
            Transform traitTransform;
            for (int i = 0; i < shopUnit.Traits.Count; i++)
            {
                traitTransform = traitsParent.GetChild(i);
                Transform iconTransform = traitTransform.Find("Icon");
                if (iconTransform != null)
                {
                    if (iconTransform.TryGetComponent(out Image iconImage))
                    {
                        iconImage.sprite = shopUnit.Traits[i].traitSprite;
                    }
                    else
                    {
                        Debug.LogWarning("Shop unit trait missing image component");
                    }
                }
                else
                {
                    Debug.LogWarning("Shop unit trait missing icon object");
                }

                traitTransform.gameObject.SetActive(true);
            }
        }
        shopUnit.gameObject.SetActive(true);
    }

    // Get a random unitData according to the probaility of getting each rarity
    private UnitData GetRandomUnitData()
    {
        if (_shopUnitsDatabase.Count > 0)
        {
            UnitRarity randomRarity = GetRandomUnitRarity();
            List<UnitData> unitsOfRarity = GetUnitsOfRarity(randomRarity);
            if (unitsOfRarity.Count > 0)
            {
                int randomIndex = Random.Range(0, unitsOfRarity.Count);
                return unitsOfRarity[randomIndex];
            }
        }
        else
        {
            Debug.LogWarning("UnitsDatabase is empty.");
        }
        return null;
    }

    // Returns a random rarity according to the probabilities
    private UnitRarity GetRandomUnitRarity()
    {
        int[] probabilities = GetCurrentLevelProbabilities();
        int totalProbability = probabilities.Sum();
        int randomNumber = Random.Range(0, totalProbability);

        int index = 0;
        int accumilatedProbablity = probabilities[0];
        for (int i = 1; i < probabilities.Length; i++)
        {
            if (randomNumber > accumilatedProbablity)
            {
                accumilatedProbablity += probabilities[i];
                index++;
            }
        }
        return (UnitRarity)index;
    }

    // Returns all units of a rarity in shopUnitDatabase
    private List<UnitData> GetUnitsOfRarity(UnitRarity rarity)
    {
        List<UnitData> unitsOfRarity = new();
        foreach (UnitData unitData in _shopUnitsDatabase)
        {
            if (unitData.Rarity == rarity)
            {
                unitsOfRarity.Add(unitData);
            }
        }
        return unitsOfRarity;
    }

    // Returns the current level probabilities based on player's level
    private int[] GetCurrentLevelProbabilities()
    {
        return _shopUnitsProbabilities[_player.Lvl - 1];
    }

    // Remove a unit from shop database
    public void AddUnitToShopDB(GameUnit gameUnit)
    {
        _shopUnitsDatabase.Add(gameUnit.UnitData);
    }

    // Add a unit to shop database
    public void RemoveUnitFromShopDB(GameUnit gameUnit)
    {
        _shopUnitsDatabase.Remove(gameUnit.UnitData);
    }

    // Buy unit from the shop destroy the gameObject
    public void BuyUnit(ShopUnit shopUnit)
    {
        if (CanAfford(shopUnit.Cost) && !_player.Bench.IsFull())
        {
            _player.PayGold(shopUnit.Cost);
            _player.Bench.CreateGameUnit(LocalPlayer.Instance, shopUnit.UnitData, 1);
            shopUnit.gameObject.SetActive(false);
            Button buyButton = shopUnit.transform.parent.gameObject.GetComponent<Button>();
            buyButton.interactable = false;
            DisableHighlight(shopUnit);
            HighlightSameUnits(shopUnit);
        }
    }

    // Highlight shop units that the player has on bench or board
    private void HighlightSameUnits(ShopUnit shopUnit)
    {
        for (int i = 0; i < SHOP_UNITS; i++)
        {
            Transform shopUnitParent = _shopUnitsTransform.GetChild(i);
            foreach (ShopUnit currShopUnit in shopUnitParent.GetComponentsInChildren<ShopUnit>())
            {
                if (currShopUnit != null)
                {
                    Transform highlighTransform = _shopUnitsTransform.GetChild(i).Find("Highlight");
                    if (_player.HasUnit(shopUnit.UnitData.UnitName) && shopUnit.UnitName == currShopUnit.UnitName)
                    {
                        highlighTransform.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
    }

    // Disable highlight for given shopUnit
    private void DisableHighlight(ShopUnit shopUnit)
    {
        Transform highlighTransform = shopUnit.transform.parent.Find("Highlight");
        highlighTransform.gameObject.SetActive(false);
    }

    // Disable highlight for all shopUnits
    private void DisableHighlights()
    {
        for (int i = 0; i < SHOP_UNITS; i++)
        {
            Transform shopUnitParent = _shopUnitsTransform.GetChild(i);
            shopUnitParent.Find("Highlight").gameObject.SetActive(false);
        }
    }

    // Sell unit and remove it from bench
    public void SellUnit(GameUnit gameUnit)
    {
        if (gameUnit.IsOnBoard)
        {
            Board.RemoveUnitFromBoard(gameUnit);
        }
        else
        {
            _player.Bench.RemoveUnitFromBench(gameUnit);
        }

        if (gameUnit.StarLevel == gameUnit.MAX_STAR_LEVEL)
        {
            AddUnitToShopDB(gameUnit);
        }

        _player.GainGold(gameUnit.Cost);
    }

    // Set unit sell field active
    public void ActivateUnitSellField(int unitCost)
    {
        // Enable the unitSellField
        if (_unitSellField != null)
        {
            _unitSellField.SetActive(true);
            Transform textTransform = _unitSellField.transform.Find("Text");
            if (textTransform != null)
            {
                if (textTransform.TryGetComponent<TextMeshPro>(out var text))
                {
                    text.text = "Sell for " + unitCost + "g";
                }
            }
            else
            {
                Debug.LogWarning("Missing text transform");
            }
        }
    }

    // Set Unit sell field disabled
    public void DisableUnitSellField()
    {
        if (_unitSellField != null)
        {
            _unitSellField.SetActive(false);
        }
    }
}
