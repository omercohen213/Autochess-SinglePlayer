using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private Transform _shopUnitsTransform;

    private readonly int SHOP_SIZE = 5;
    private readonly int REROLL_COST = 1;
    private readonly int XP_COST = 4;
    private Player _player;

    [SerializeField] private UnitsDatabase _unitsDatabase;

    private void Start()
    {
        _player = Player.Instance;
        RerollUnits();
    }

    // Buy unit from the shop and set it inactive
    public void BuyUnit(int pos)
    {
        GameObject unitGo = _shopUnitsTransform.GetChild(pos).GetChild(0).gameObject;
        Unit unit = unitGo.GetComponent<Unit>();
        if (CanAfford(unit.Cost) && _player.UnitsBench.IsFull() == false)
        {
            _player.PayGold(unit.Cost);
            _player.UnitsBench.AddUnit(unit);
            unitGo.SetActive(false);
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
        for (int i = 0; i < _shopUnitsTransform.childCount; i++)
        {
            _shopUnitsTransform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }

        // Randomly select new units from the UnitsDatabase
        for (int i = 0; i < _shopUnitsTransform.childCount; i++)
        {
            UnitData randomUnitData = GetRandomUnitData();
            GameObject unitGo = _shopUnitsTransform.GetChild(i).GetChild(0).gameObject;
            if (randomUnitData != null)
            {
                unitGo.GetComponent<Unit>().SetUnitData(randomUnitData);             

                // Update the visual representation
                TextMeshProUGUI shopUnitHp = unitGo.transform.Find("Hp").GetComponent<TextMeshProUGUI>();
                shopUnitHp.text = unitGo.GetComponent<Unit>().Hp.ToString();

                // Make the shop unit active
                unitGo.SetActive(true);
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
