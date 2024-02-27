using System;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    Preparation,
    Battle,
    BattleResult,
    GameOver
}

public class GameManager : MonoBehaviour
{
    private Player _locaclPlayer;
    
    public GamePhase currentPhase;
    public event Action<GamePhase> OnPhaseChanged;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

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
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        _locaclPlayer = LocalPlayer.Instance;
        SwitchToPhase(GamePhase.Preparation);
    }

    public void SwitchToPhase(GamePhase gamePhase)
    {
        currentPhase = gamePhase;
        OnPhaseChanged?.Invoke(currentPhase);

        switch (currentPhase)
        {
            case GamePhase.Preparation:
                break;
            case GamePhase.Battle:
                //Debug.Log("Starting battle...");
                break;
            case GamePhase.BattleResult:
                ShowBattleResult();
                break;
            default:
                Debug.LogError("Unknown game phase!");
                break;
        }
    }

    public void SwitchToPhase(string str)
    {
        if (str == GamePhase.Battle.ToString())
        {
            SwitchToPhase(GamePhase.Battle);
        }
    }

    // Shows battle result after each battle
    private void ShowBattleResult()
    {
        if (_locaclPlayer.HasAnyUnitOnBoard())
        {
            Debug.Log("Player won!");
            SwitchToPhase(GamePhase.Preparation);
        }
        else
        {
            _locaclPlayer.Lives--;
            Debug.Log("Opponent won! " + _locaclPlayer.Lives + " lives left!");

            if (_locaclPlayer.Lives <= 0)
            {
                SwitchToPhase(GamePhase.GameOver);
            }
            else
            {
                SwitchToPhase(GamePhase.Preparation);
            }
        }
    }
}
