using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public enum RoundState
{
    Preparation,
    Battle,
    RoundOver
}

public class RoundManager : MonoBehaviour
{
    private Player _localPlayer;
    private Player _opponent;

    private List<(GameUnit Unit, Hex Hex)> _playerUnitsHexes;
    private List<(GameUnit Unit, Hex Hex)> _opponentUnitsHexes;

    private RoundState _currentRoundState;
    private int _currentPhase;
    private int _currentStage;
    private readonly int STAGES_PER_PHASE = 4;

    private static RoundManager _instance;
    public static RoundManager Instance => _instance;

    public int CurrentStage { get => _currentStage; set => _currentStage = value; }
    public int CurrentPhase { get => _currentPhase; set => _currentPhase = value; }
    public RoundState CurrentRoundState { get => _currentRoundState; set => _currentRoundState = value; }

    public RoundState _currentState;

    public event Action<RoundState> OnRoundStateChanged;

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
        _opponent = Opponent.Instance;
        _currentPhase = 1;
        _currentStage = 0;
        ChangeRoundState(RoundState.Preparation);
    }

    // Switch to given game phase and invoke the event
    public void ChangeRoundState(RoundState roundState)
    {
        _currentState = roundState;
        OnRoundStateChanged?.Invoke(_currentState);

        switch (_currentState)
        {
            case RoundState.Preparation:
                UpdateRoundNumber();
                break;
            case RoundState.Battle:
                GetFightingUnits();
                break;
            case RoundState.RoundOver:
                if (!CheckForGameOver())
                {
                    ShowBattleResult();
                    CreateNextStage();
                    ReplaceUnitsOnBoard();
                }
                break;
            default:
                Debug.LogError("Unknown game phase!");
                break;
        }
    }


    public void HandleNoUnitsLeft(Player player)
    {
        Debug.Log(_currentRoundState);
        if (_currentRoundState == RoundState.Battle)
        {
            ChangeRoundState(RoundState.RoundOver);
        }
    }

    private bool CheckForGameOver()
    {
        return false;
    }

    private void CreateNextStage()
    {
    }

    // Place units that fought back on board 
    private void ReplaceUnitsOnBoard()
    {
        _localPlayer.ReenableUnits(_playerUnitsHexes);
        Board.PlaceUnitsOnBoard(_playerUnitsHexes);
    }

    // Get the fighting units and their initial hex on board
    private void GetFightingUnits()
    {
        _playerUnitsHexes = new(_localPlayer.GetBoardUnitsHexes());
        _opponentUnitsHexes = new(_opponent.GetBoardUnitsHexes());
    }

    private void UpdateRoundNumber()
    {
        // Phase is over, start new one
        if (_currentStage >= STAGES_PER_PHASE)
        {
            _currentPhase += 1;
            _currentStage = 1;
        }
        else
        {
            _currentStage += 1;
        }
    }

    public void SwitchToBattleState()
    {
        _currentRoundState = RoundState.Battle;
        ChangeRoundState(RoundState.Battle);
    }

    // Shows battle result after each battle
    private void ShowBattleResult()
    {
        if (_localPlayer.HasAnyUnitOnBoard())
        {
            Debug.Log("Player won!");
        }
        else
        {
            _localPlayer.Lives--;
            Debug.Log("Opponent won! " + _localPlayer.Lives + " lives left!");

            if (_localPlayer.Lives <= 0)
            {
                // Game is over
                return;
            }
        }
        ChangeRoundState(RoundState.Preparation);
    }
}
