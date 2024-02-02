using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private TextMeshProUGUI _lvlText;
    [SerializeField] private TraitTracker _traitTracker;

    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new();
                    _instance = singletonObject.AddComponent<UIManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private void Start()
    {
        UpdateGoldUI();
        UpdateXpUI();
    }

    // Update current gold text
    public void UpdateGoldUI()
    {
        _goldText.text = Player.Instance.Gold.ToString() + "$";
    }

    // Update current xp text
    public void UpdateXpUI()
    {
        _xpText.text = Player.Instance.Xp.ToString() + "/" + GameManager.Instance.GetXpToLevelUp(Player.Instance.Lvl).ToString();
    }

    // Update current lvl text
    public void UpdatePlayerLvlUI()
    {
        _lvlText.text = Player.Instance.Lvl.ToString();
    }


    public void UpdateTraitUI(Trait trait, int currentStage, int unitCount, int lastUnitCount)
    {
        if (unitCount == 1 && lastUnitCount == 0)
        {
            _traitTracker.AddTrait(trait, currentStage, unitCount);
        }
    }

    
}
