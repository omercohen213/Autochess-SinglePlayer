using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameUnitAttack : MonoBehaviour
{
    private GameUnit _gameUnit;

    private bool _isAttacking = false;
    private GameUnit _currentTarget;

    private GameUnitBehavior _gameUnitBehavior;

    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }

    public delegate void AttackEventHandler(GameUnit attacker, GameUnit target, int damage);
    public static event AttackEventHandler OnAttack;

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
        _gameUnitBehavior = GetComponent<GameUnitBehavior>();
    }


    public void Attack(GameUnit target)
    {
        _isAttacking = true;
        if (target != null)
        {
            StartCoroutine(AttackCoroutine(target));
        }
        else
        {
            Debug.Log("target is null");
        }
    }

    private IEnumerator AttackCoroutine(GameUnit target)
    {
        while (target.Hp > 0) // ****** object is destroyed
        {
            yield return new WaitForSeconds(1f / _gameUnit.AttackSpeed);
            _currentTarget = target;

            OnAttack?.Invoke(_gameUnit, _currentTarget, _gameUnit.AttackDamage); // InvokeAttackEvent(_gameUnit, _currentTarget, _gameUnit.AttackDamage);
            //Debug.Log(_gameUnit.UnitName + " attacked " + _currentTarget.UnitName + " -" + _gameUnit.AttackDamage);

            // Stop the coroutine if the next attack kills the target because target will then be null after destroyed
            if (target.IsDead())
            {
                _isAttacking = false;
                Debug.Log("enemy killed: " + target);
                _gameUnitBehavior.UpdatePathfinding();
                break;
            }
        }
    }

    public static void InvokeAttackEvent(GameUnit attacker, GameUnit target, int damage)
    {
        if (OnAttack != null)
        {
            OnAttack(attacker, target, damage);
        }
    }


}
