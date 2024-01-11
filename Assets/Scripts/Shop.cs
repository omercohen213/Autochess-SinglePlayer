using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject _shopUnitPrefab;
    [SerializeField] private Transform _shopUnitsParent;

    private GameObject[] _shopUnitsGo;
    private readonly int SHOP_SIZE = 5;
    private readonly int REROLL_COST = 1;
    private readonly int XP_COST = 4;
    private Player _player;

    [SerializeField] private UnitsDatabase _unitsDatabase;

    private void Start()
    {
        _player = Player.Instance;
        _shopUnitsGo = new GameObject[SHOP_SIZE];

        // Add shop units objects and call BuyUnit on click
        for (int i = 0; i < SHOP_SIZE; i++)
        {
            _shopUnitsGo[i] = Instantiate(_shopUnitPrefab, _shopUnitsParent.GetChild(i).transform);
            TextMeshProUGUI shopUnitHp = _shopUnitsGo[i].transform.Find("Hp").GetComponent<TextMeshProUGUI>();
            shopUnitHp.text = _shopUnitsGo[i].GetComponent<Unit>().Hp.ToString();

            int currentIndex = i;
            _shopUnitsGo[i].GetComponent<Button>().onClick.AddListener(() => BuyUnit(currentIndex));
        }
    }

    // Buy unit from the shop and set it inactive
    public void BuyUnit(int pos)
    {
        // check bench limit
        Unit unit = _shopUnitsGo[pos].GetComponent<Unit>();
        if (CanAfford(unit.Cost))
        {
            _player.PayGold(unit.Cost);
            _player.UnitsBench.AddUnit(unit);
            _shopUnitsGo[pos].SetActive(false);
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
        if (_player.Gold + goldToPay > 0)
            return true;
        else
        {
            Debug.Log("Cannot afford!");
            return false;
        }
    }

    // Show new units in shop, randomly chosen from the units database 
    private void RerollUnits()
    {
        // Clear current shop units
        for (int i = 0; i < SHOP_SIZE; i++)
        {
            _shopUnitsGo[i].SetActive(false);
        }

        // Randomly select new units from the UnitsDatabase
        for (int i = 0; i < SHOP_SIZE; i++)
        {
            UnitData randomUnitData = GetRandomUnitData();
            if (randomUnitData != null)
            {
                _shopUnitsGo[i].GetComponent<Unit>().SetUnitData(randomUnitData);

                // Update the visual representation
                TextMeshProUGUI shopUnitHp = _shopUnitsGo[i].transform.Find("Hp").GetComponent<TextMeshProUGUI>();
                shopUnitHp.text = _shopUnitsGo[i].GetComponent<Unit>().Hp.ToString();

                // Make the shop unit active
                _shopUnitsGo[i].SetActive(true);
            }
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
