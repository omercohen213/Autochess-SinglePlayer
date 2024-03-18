using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    protected int _level;
    protected GameObject _abilityObjectPrefab;
    [SerializeField] protected Animator _animator;

    public virtual void CastAbility(GameUnit caster, GameUnit target)
    {
        _level = caster.StarLevel;
    }

    public virtual void AnitmateAbility(GameUnit target)
    {
        //_animator.SetTrigger("FireArrow");
        //_abilityObjectPrefab.transform.position = target.transform.position;        
    }
}
