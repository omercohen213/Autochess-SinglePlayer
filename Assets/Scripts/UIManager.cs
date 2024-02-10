using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Player _player;

    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private TextMeshProUGUI _lvlText;
    [SerializeField] private TextMeshProUGUI _boardLimitText;
    [SerializeField] private TraitTrackerUI _traitTracker;

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
        _player = LocalPlayer.Instance;        
        UpdateGoldUI();
        UpdateXpUI();
        UpdateBoardLimit();
    }

    // Update current gold text
    public void UpdateGoldUI()
    {
        _goldText.text = _player.Gold.ToString();
    }

    // Update current xp text
    public void UpdateXpUI()
    {
        _xpText.text = _player.Xp.ToString() + "/" 
            + GameManager.Instance.GetXpToLevelUp(_player.Lvl).ToString();
    }

    // Update current lvl text
    public void UpdatePlayerLvlUI()
    {
        _lvlText.text = _player.Lvl.ToString();
    }


    public void UpdateTraitUI(Trait trait, int currentStage, int unitCount, int lastUnitCount)
    {
        _traitTracker.UpdateTraits(trait, currentStage, unitCount, lastUnitCount);
    }

    public void UpdateBoardLimit()
    {
        _boardLimitText.text = _player.BoardUnits.Count + " / " + _player.boardLimit;
        if (_player.IsBoardLimitReached())
        {
            _boardLimitText.color = Color.red;
        }
        else
        {
            _boardLimitText.color = Color.green;
        }
    }
}
