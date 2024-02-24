using System;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    Preparation,
    Battle,
    BattleWon,
    BattleLost,
}

public class GameManager : MonoBehaviour
{
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
        // Start with the preparation phase
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
                //Debug.Log("Starting next preparation phase...");
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
}
