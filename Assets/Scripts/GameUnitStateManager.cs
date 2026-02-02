using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameUnit;

public enum UnitState
{
    Idle,
    Moving,
    Attacking,
    Dead
}

// Class to store and change the state of a unit during the battle phase
public class GameUnitStateManager : MonoBehaviour
{
    private GameUnit _gameUnit;
    private GameUnitAttack _attack;
    private GameUnitMovement _movement;

    private UnitState _currentState;
    public UnitState CurrentState { get => _currentState; set => _currentState = value; }

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
        _attack = GetComponent<GameUnitAttack>();
        _movement = GetComponent<GameUnitMovement>();
    }

    private void OnEnable()
    {
        _currentState = UnitState.Idle;
        _attack.OnAttackFinished += OnAttackFinished;
        _movement.OnMovementFinished += OnMovementFinished;
    }

    private void OnDisable()
    {
        _attack.OnAttackFinished -= OnAttackFinished;
        _movement.OnMovementFinished -= OnMovementFinished;
    }

    public bool RequestAttack()
    {
        if (_currentState != UnitState.Idle)
        {
            Debug.Log("Cannot change to requested state from current state");
            return false;
        }
        _currentState = UnitState.Attacking;
        _attack.Attack(_gameUnit.CurrentTarget);
        return true;
    }

    public bool RequestMove()
    {
        if (_currentState != UnitState.Idle && _currentState != UnitState.Moving)
        {
            Debug.Log("Cannot change to requested state from current state");
            return false;
        }
        _currentState = UnitState.Moving;
        return true;
    }

    public bool RequestDead() {

        _currentState = UnitState.Dead;
        return true;
    }

    public void OnAttackFinished()
    {
        _currentState = UnitState.Idle;
    }

    private void OnMovementFinished()
    {
        _currentState = UnitState.Idle;
    }
}
