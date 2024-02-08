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
    private List<UnitData> _shopUnitDatabase;



    private List<int[]> _shopUnitsProbabilities;
    private readonly int[] LVL1_PROBABILITIES = new[] { 70, 30, 0, 0, 0 };
    private readonly int[] LVL2_PROBABILITIES = new[] { 55, 30, 15, 0, 0 };
    private readonly int[] LVL3_PROBABILITIES = new[] { 45, 33, 20, 2, 0 };
    private readonly int[] LVL4_PROBABILITIES = new[] { 30, 40, 25, 5, 0 };
    private readonly int[] LVL5_PROBABILITIES = new[] { 19, 30, 35, 10, 1 };

    private readonly int REROLL_COST = 1;
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
    }

    private void Start()
    {
        _player = LocalPlayer.Instance;
        _shopUnitDatabase = new();
        foreach (UnitData unitData in UnitsDatabase.Instance.Units)
        {
            _shopUnitDatabase.Add(unitData);
        }

        if (_unitSellField != null)
        {
            _unitSellField.SetActive(false);
        }
        _shopUnitsProbabilities = new() { LVL1_PROBABILITIES, LVL2_PROBABILITIES, LVL3_PROBABILITIES, LVL4_PROBABILITIES, LVL5_PROBABILITIES };

        RerollUnits();

    }

    // Buy unit from the shop and set it inactive
    public void BuyUnit(int pos)
    {
        GameObject shopUnitGo = _shopUnitsTransform.GetChild(pos).GetChild(0).gameObject;
        ShopUnit shopUnit = shopUnitGo.GetComponent<ShopUnit>();
        if (CanAfford(shopUnit.Cost) && !_player.Bench.IsFull())
        {
            _player.PayGold(shopUnit.Cost);
            _player.Bench.CreateGameUnit(shopUnit.UnitData.Id, 1);
            shopUnitGo.SetActive(false);
        }
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
        for (int i = 0; i < _shopUnitsTransform.childCount; i++)
        {
            _shopUnitsTransform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }

        // Randomly select new units from the UnitsDatabase
        for (int i = 0; i < _shopUnitsTransform.childCount; i++)
        {
            UnitData unitData = GetRandomUnitData();
            Transform shopUnitParent = _shopUnitsTransform.GetChild(i);
            GameObject shopUnitGo = shopUnitParent.GetChild(0).gameObject;
            if (unitData != null)
            {
                shopUnitGo.GetComponent<ShopUnit>().SetUnitData(unitData.Id);

                // Update the visual representation
                TextMeshProUGUI shopUnitCost = shopUnitGo.transform.Find("Cost").GetComponent<TextMeshProUGUI>();
                shopUnitCost.text = shopUnitGo.GetComponent<ShopUnit>().Cost.ToString();
                Image shopUnitImage = shopUnitGo.transform.GetComponent<Image>();
                shopUnitImage.sprite = unitData.ShopImage;

                // Make the shop unit gameobject active
                shopUnitGo.SetActive(true);
            }
        }
    }

    // Get a random unitData according to the probaility of getting each rarity
    private UnitData GetRandomUnitData()
    {
        if (_shopUnitDatabase.Count > 0)
        {
            UnitRarity randomRarity = GetRandomUnitRarity();
            List<UnitData> unitsOfRarity = GetUnitsOfRarity(randomRarity);
            int randomIndex = Random.Range(0, unitsOfRarity.Count);
            Debug.Log("index " + randomIndex+ " count " + unitsOfRarity.Count);
            if (unitsOfRarity != null)
            {
                return unitsOfRarity[randomIndex];
            }
            else
            {
                Debug.LogError("unitsOfRarity" + randomRarity + " is null or empty.");
                return null;
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
        return(UnitRarity)index;
    }

    // Returns all units of a rarity in shopUnitDatabase
    private List<UnitData> GetUnitsOfRarity(UnitRarity rarity)
    {
        List<UnitData> unitsOfRarity = new();
        foreach (UnitData unitData in _shopUnitDatabase)
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
        return _player.Lvl switch
        {
            1 => LVL1_PROBABILITIES,
            2 => LVL2_PROBABILITIES,
            3 => LVL3_PROBABILITIES,
            4 => LVL4_PROBABILITIES,
            5 => LVL5_PROBABILITIES,
            _ => null,
        };
    }

    // Remove a unit from shop database
    public void AddUnitToShopDB(UnitData unitData)
    {
        _shopUnitDatabase.Add(unitData);
    }

    // Add a unit to shop database
    public void RemoveUnitFromShopDB(UnitData unitData)
    {
        _shopUnitDatabase.Remove(unitData);
    }

    // Sell unit and remove it from bench
    public void SellUnit(GameUnit unit)
    {
        if (unit.IsOnBoard)
        {
            Board.Instance.RemoveUnitFromBoard(unit);
        }
        else
        {
            _player.Bench.RemoveUnitFromBench(unit);
        }

        if (unit.StarLevel == unit.MAX_STAR_LEVEL)
        {
            AddUnitToShopDB(unit.UnitData);
        }

        _player.GainGold(unit.Cost);
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
