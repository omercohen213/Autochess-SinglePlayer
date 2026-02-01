using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class UIManager : MonoBehaviour
{
    Player _player;

    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private Slider _xpBar;
    [SerializeField] private TextMeshProUGUI _playerLvlText;
    [SerializeField] private TextMeshProUGUI _roundLvlText;
    [SerializeField] private TextMeshProUGUI _boardLimitText;
    [SerializeField] private TraitTrackerUI _traitTracker;
    [SerializeField] private GameObject _attackButton;

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

    private void OnEnable()
    {
        RoundManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDisable()
    {
        RoundManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void Start()
    {
        _player = LocalPlayer.Instance;
        UpdateGoldUI();
        UpdateXpUI();
        UpdatePlayerLvlUI();
        UpdateBoardLimit();
    }

    public void OnPhaseChanged(GamePhase newPhase)
    {
        switch (newPhase)
        {
            case GamePhase.Preparation:
                ShowUIElement(_attackButton);
                break;
            case GamePhase.RoundStart:
                HideUIElement(_attackButton);
                break;
        }
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
            + _player.GetXpToLevelUp(_player.Lvl).ToString();
        UpdateXPBarUI();
    }


    private void UpdateXPBarUI()
    {
        float currentXP = _player.Xp;
        float xpToLevelUp = _player.GetXpToLevelUp(_player.Lvl);
        float fillAmount = currentXP / (currentXP + xpToLevelUp);
        _xpBar.value = fillAmount;
    }

    // Update current lvl text
    public void UpdatePlayerLvlUI()
    {
        _playerLvlText.text = _player.Lvl.ToString();
        UpdateXpUI();
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

    // Hide given UI element gameobject
    public void HideUIElement(GameObject elementToHide)
    {
        elementToHide.SetActive(false);
    }

    // Show given hidden UI element gameobject
    public void ShowUIElement(GameObject elementToHide)
    {
        elementToHide.SetActive(true);
    }
}
