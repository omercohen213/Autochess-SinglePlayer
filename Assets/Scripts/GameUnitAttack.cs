using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameUnit))]
public class GameUnitAttack : MonoBehaviour
{
    private GameUnit _gameUnit;

    private bool _firstAttack = true;


    // Keep coroutine reference so we don't start duplicates and can stop on disable
    private Coroutine _attackCoroutine;
    public delegate void AttackEventHandler(GameUnit attacker, GameUnit target, int damage);
    public static event AttackEventHandler OnAttack;
    public static event Action<GameUnit> OnAttackFinished; // fired when this unit finishes attacking


    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
    }

    public void Attack(GameUnit target)
    {
        if (target == null)
        {
            Debug.LogWarning("GameUnitAttack.Attack: target is null");
            return;
        }

        // If already attacking, ignore new requests
        if (_gameUnit.CurrentState == GameUnit.UnitState.Attacking) return;

        _gameUnit.CurrentTarget= target;

        // Unit is dead, finish immediately
        if (_gameUnit.CurrentTarget.IsDead() || _gameUnit.IsDead())
        {
            _firstAttack = true;
            return;
        }

        _gameUnit.ChangeState(GameUnit.UnitState.Attacking);
        _attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        if (_firstAttack)
        {
            _firstAttack = false;
            OnAttack?.Invoke(_gameUnit, _gameUnit.CurrentTarget, _gameUnit.AttackDamage);
        }

        while (!_gameUnit.CurrentTarget.IsDead() && !_gameUnit.IsDead())
        {
            yield return new WaitForSeconds(1f / _gameUnit.AttackSpeed);

            // Target or attacker may die while waiting
            if (_gameUnit.CurrentTarget.IsDead() || _gameUnit.IsDead()) break;

            OnAttack?.Invoke(_gameUnit, _gameUnit.CurrentTarget, _gameUnit.AttackDamage);
        }

        // Attack loop finished
        _gameUnit.ChangeState(GameUnit.UnitState.None);

        _firstAttack = true;
        _attackCoroutine = null;
    }

    private void OnDisable()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }

        _gameUnit.ChangeState(GameUnit.UnitState.None);
        _firstAttack = true;
    }

    private void OnDestroy()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }
}
