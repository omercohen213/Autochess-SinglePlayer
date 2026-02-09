using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GameUnit))]
public class GameUnitAttack : MonoBehaviour
{
    private GameUnit _gameUnit;

    private bool _firstAttack;


    // Keep coroutine reference so we don't start duplicates and can stop on disable
    private Coroutine _attackCoroutine;
    public event Action OnAttackFinished;


    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
    }

    private void Start()
    {
        _firstAttack = true;
    }

    private void OnDisable()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }

        OnAttackFinished?.Invoke();
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

    public void StartAttack(GameUnit target)
    {
        if (target == null)
        {
            Debug.LogWarning("GameUnitAttack.Attack: target is null");
            return;
        }

        _gameUnit.CurrentTarget = target;

        // Unit is dead, finish immediately
        if (_gameUnit.CurrentTarget.IsDead() || _gameUnit.IsDead())
        {
            _firstAttack = true;
            return;
        }

        _attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        if (_firstAttack)
        {
            _firstAttack = false;
            Attack(_gameUnit, _gameUnit.CurrentTarget, _gameUnit.AttackDamage);
        }

        while (!_gameUnit.CurrentTarget.IsDead() && !_gameUnit.IsDead())
        {
            yield return new WaitForSeconds(1f / _gameUnit.AttackSpeed);

            // Target or attacker may die while waiting
            if (_gameUnit.CurrentTarget.IsDead() || _gameUnit.IsDead()) break;

            Attack(_gameUnit, _gameUnit.CurrentTarget, _gameUnit.AttackDamage);
        }

        // Attack loop finished
        OnAttackFinished?.Invoke();

        _firstAttack = true;
        _attackCoroutine = null;
    }

    public void Attack(GameUnit attacker, GameUnit target, int damage)
    {
        bool isCritical = IsCriticalAttack(attacker);
        if (isCritical)
        {
            damage = Mathf.RoundToInt(damage * attacker.CritDamage);
        }

        if (attacker.Mp == attacker.MaxMp)
        {
            if (attacker.Ability != null)
            {
                attacker.Ability.CastAbility(attacker, target);
            }
            attacker.Mp = 0;
        }
        else
        {
            attacker.Mp += 10;
        }
        attacker.GameUnitUI.UpdateUnitMPBar();

        // Meele - no projectile
        if (IsMelee(attacker))
        {
            target.OnDamageTaken(damage, false, isCritical);
        }
        else
        {
            attacker.ShootProjectile(attacker.WeaponProjectilePrefab, target, damage, false, isCritical);
        }
        attacker.AnimationController.AnimateAttack();
    }

    public bool IsMelee(GameUnit gameUnit)
    {
        return (gameUnit.Weapon == Weapon.MeeleOneHanded) || (gameUnit.Weapon == Weapon.MeeleTwoHanded) || (gameUnit.Weapon == Weapon.NoWeapon);
    }

    private bool IsCriticalAttack(GameUnit gameUnit)
    {
        return Random.value < gameUnit.CritChance;
    }
}
