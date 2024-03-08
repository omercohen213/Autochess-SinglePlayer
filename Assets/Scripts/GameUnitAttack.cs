using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class GameUnitAttack : MonoBehaviour
{
    private GameUnit _gameUnit;

    [SerializeField] private bool _isAttacking = false;
    private bool _firstAttack = true;
    private GameUnit _currentTarget;

    private GameUnitMovement _gameUnitBehavior;

    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }

    public delegate void AttackEventHandler(GameUnit attacker, GameUnit target, int damage);
    public static event AttackEventHandler OnAttack;

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
        _gameUnitBehavior = GetComponent<GameUnitMovement>();
    }

    public void Attack(GameUnit target)
    {
        if (target != null)
        {
            _currentTarget = target;
            if (_currentTarget.IsDead() || _gameUnit.IsDead())
            {
                _isAttacking = false;
                _firstAttack = true;
                _gameUnitBehavior.UpdatePathfinding();
                return;
            }
            _isAttacking = true;
            StartCoroutine(AttackCoroutine());
        }
        else
        {
            Debug.Log("target is null");
        }
    }

    private IEnumerator AttackCoroutine()
    {
        // If it's the first attack, immediately invoke the attack event without waiting
        if (_firstAttack)
        {
            _firstAttack = false;
            OnAttack?.Invoke(_gameUnit, _currentTarget, _gameUnit.AttackDamage); // task?
        }

        while (!_currentTarget.IsDead() && !_gameUnit.IsDead())
        {
            yield return new WaitForSeconds(1f / _gameUnit.AttackSpeed);
            OnAttack?.Invoke(_gameUnit, _currentTarget, _gameUnit.AttackDamage);
        }
        if (!_gameUnit.IsDead())
        {
            _gameUnitBehavior.UpdatePathfinding();
        }
        _isAttacking = false;
        _firstAttack = true;
    }
}
