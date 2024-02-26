using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameUnitAttack : MonoBehaviour
{
    private GameUnit _currentTarget;
    private GameUnit _gameUnit;
    private Animator _animator;

    public delegate void DamageEventHandler(GameUnit attacker, GameUnit target, int damage);
    public static event DamageEventHandler OnDamageReceived;

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();

        Transform animationTransform = transform.Find("Animation");
        if (animationTransform != null)
        {
            _animator = animationTransform.GetComponent<Animator>();
        }
        else
        {
            Debug.LogWarning("Missing animation transform on game unit " + _gameUnit.UnitName);
        }
    }

    public static void InvokeDamageEvent(GameUnit attacker, GameUnit target, int damage)
    {
        if (OnDamageReceived != null)
        {
            OnDamageReceived(attacker, target, damage);
        }
    }

    public void Attack(GameUnit target)
    {
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
            yield return new WaitForSeconds(_gameUnit.AttackSpeed);
            _currentTarget = target;
            if (_animator != null)
            {
                _animator.SetTrigger("Cast0");
            }
            InvokeDamageEvent(_gameUnit, _currentTarget, _gameUnit.AttackDamage);
            Debug.Log(_gameUnit.UnitName + " attacked " + _currentTarget.UnitName + " -" + _gameUnit.AttackDamage);
        }
    }
}
