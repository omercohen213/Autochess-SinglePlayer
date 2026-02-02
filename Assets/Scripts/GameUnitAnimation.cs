using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnitAnimation : MonoBehaviour
{
    private GameUnit _gameUnit;
    private Animator _animator;

    private void Awake()
    {
        _gameUnit = GetComponent<GameUnit>();
    }

    private void OnEnable()
    {
        _gameUnit.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        _gameUnit.OnDeath -= OnDeath;
    }

    private void OnDeath(GameUnit gameUnit)
    {
        AnimateDeath();
    }

    public void InitializeAnimator()
    {

        if (_gameUnit.Owner == LocalPlayer.Instance)
        {
            Transform animationTransform = transform.Find("Character");
            if (animationTransform != null)
            {
                _animator = animationTransform.GetComponent<Animator>();
            }
            else
            {
                Debug.LogWarning("Missing animation transform on game unit " + _gameUnit.UnitName);
            }
        }
        // Monster animation
        else
        {
            _animator = GetComponent<Animator>();
        }
    }

    public void AnimateAttack()
    {
        if (_animator != null)
        {
            _animator.SetFloat("Speed", _gameUnit.AttackSpeed);
            switch (_gameUnit.Weapon)
            {
                case Weapon.MeeleOneHanded:
                    _animator.SetTrigger("Slash1H");
                    break;
                case Weapon.MeeleTwoHanded:
                    _animator.SetTrigger("Slash2H");
                    break;
                case Weapon.Staff:
                    // todo: change to jab if close range
                    _animator.SetTrigger("Cast");
                    break;
                case Weapon.Bow:
                    _animator.SetTrigger("SimpleBowShot");
                    // BowShotAnimation();
                    break;
                case Weapon.Gun:
                    break;
                case Weapon.NoWeapon:
                    _animator.SetTrigger("Attack");
                    break;
            }
        }
    }

    public void AnimateDeath()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Death");
        }
    }

    public void AnimateMovement()
    {
        if (_animator != null)
        {
            _animator.SetBool("Walk", true);
        }
    }

    public void StopAnimateMovement()
    {
        if (_animator != null)
        {
            _animator.SetBool("Walk", false);
        }
    }
}
