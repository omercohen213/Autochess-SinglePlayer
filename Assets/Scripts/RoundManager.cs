using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GamePhase
{
    Preparation,
    RoundStart,
    RoundOver,
    GameOver
}

public class RoundManager : MonoBehaviour
{
    private Player _localPlayer;
    private bool _roundOverTriggered;

    public GamePhase currentPhase;
    public event Action<GamePhase> OnPhaseChanged;

    private static RoundManager _instance;
    public static RoundManager Instance => _instance;

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
        _localPlayer = LocalPlayer.Instance;
        SwitchToPhase(GamePhase.Preparation);
    }

    // Switch to given game phase and invoke the event
    public void SwitchToPhase(GamePhase gamePhase)
    {
        currentPhase = gamePhase;
        OnPhaseChanged?.Invoke(currentPhase);

        switch (currentPhase)
        {
            case GamePhase.Preparation:
                break;
            case GamePhase.RoundStart:
                //Debug.Log("Starting battle...");
                break;
            case GamePhase.RoundOver:
                ShowBattleResult();
                break;
            case GamePhase.GameOver:
                GameManager.Instance.EndGame();
                break;
            default:
                Debug.LogError("Unknown game phase!");
                break;
        }
    }

    // Overload method to change phase with string (for UI elements click)
    public void SwitchToPhase(string phaseStr)
    {
        if (phaseStr == GamePhase.RoundStart.ToString())
        {
            SwitchToPhase(GamePhase.RoundStart);
            return;
        }

        // Warn if string name is wrong
        Debug.LogWarning("Unkown game phase ");

    }

    // Shows battle result after each battle
    private void ShowBattleResult()
    {
        if (_localPlayer.HasAnyUnitOnBoard())
        {
            Debug.Log("Player won!");
            SwitchToPhase(GamePhase.Preparation);
        }
        else
        {
            _localPlayer.Lives--;
            Debug.Log("Opponent won! " + _localPlayer.Lives + " lives left!");

            if (_localPlayer.Lives <= 0)
            {
                SwitchToPhase(GamePhase.GameOver);
            }
            else
            {
                SwitchToPhase(GamePhase.Preparation);
            }
        }
    }

    // When a unit finished attacking 
    public void NotifyUnitFinished()
    {
        if (_roundOverTriggered)
            return;

        /*if (NoEnemiesRemaining())
        {
            _roundOverTriggered = true;
            SwitchToPhase(GamePhase.RoundOver);
        }*/
    }
}
