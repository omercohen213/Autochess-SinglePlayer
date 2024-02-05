using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private Transform _shopUnitsTransform;
    [SerializeField] private UnitsDatabase _unitsDatabase;
    [SerializeField] private GameObject _gameUnitPrefab; // For the instantiation when unit is bought

    //private readonly int SHOP_SIZE = 5;
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
        RerollUnits();
        if (_unitSellField != null)
        {
            _unitSellField.SetActive(false);
        }
    }

    // Buy unit from the shop and set it inactive
    public void BuyUnit(int pos)
    {
        GameObject shopUnitGo = _shopUnitsTransform.GetChild(pos).GetChild(0).gameObject;
        ShopUnit shopUnit = shopUnitGo.GetComponent<ShopUnit>();
        if (CanAfford(shopUnit.Cost) && !_player.Bench.IsFull())
        {
            _player.PayGold(shopUnit.Cost);
            GameUnit unit = CreateGameUnit(shopUnit.UnitData.Id);
            _player.Bench.AddUnitToBench(unit);
            shopUnitGo.SetActive(false);
        }
    }

    // Create an instance of a unit on scene and set its data according to the id
    private GameUnit CreateGameUnit(int id)
    {
        Transform benchTransform = _player.Bench.transform;
        GameObject unitGo = Instantiate(_gameUnitPrefab, benchTransform);
        GameUnit gameUnit = unitGo.GetComponent<GameUnit>();
        gameUnit.SetUnitData(id);
        SpriteRenderer unitSpriteRenderer = gameUnit.GetComponent<SpriteRenderer>();
        unitSpriteRenderer.sprite = gameUnit.UnitSprite;
        return gameUnit;
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
        _player.GainGold(unit.Cost);
        Debug.Log("Sold for + " + unit.Cost);
    }

    // Set unit sell field active
    public void ActivateUnitSellField()
    {
        // Enable the unitSellField
        if (_unitSellField != null)
        {
            _unitSellField.SetActive(true);
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

    // Get a random UnitData from the UnitsDatabase
    private UnitData GetRandomUnitData()
    {
        if (_unitsDatabase.Units.Count > 0)
        {
            int randomIndex = Random.Range(0, _unitsDatabase.Units.Count);
            return _unitsDatabase.Units[randomIndex];
        }
        else
        {
            Debug.LogWarning("UnitsDatabase is empty.");
            return null;
        }
    }
}
