using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class FireArrow : Ability
{
    [SerializeField] private GameObject _fireArrowPrefab;
    [SerializeField] private GameObject _fireBowPrefab;

    public override void CastAbility(GameUnit caster, GameUnit target)
    {
        base.CastAbility(caster, target);
        AnitmateAbility(caster);
        caster.ShootProjectile(_fireArrowPrefab, target, 200, true, false); // change damage
    }

    public override void AnitmateAbility(GameUnit caster)
    {
        Vector3 startingPosition = new(caster.transform.position.x + 0.8f, caster.transform.position.y + 0.5f);
        GameObject fireBowGo = Instantiate(_fireBowPrefab, startingPosition, Quaternion.identity);
        fireBowGo.transform.position = startingPosition;
        _animator = fireBowGo.GetComponent<Animator>();
        _animator.SetTrigger("FireArrow");
    }


}
