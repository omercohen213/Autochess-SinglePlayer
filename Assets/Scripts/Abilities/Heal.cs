using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Ability
{
    private int[] _healPowers;

    private void Awake()
    {
        _healPowers = new int[] { 10, 20, 50 };
    }

    // Heal the ally with least hp
    public override void CastAbility(GameUnit caster, GameUnit target)
    {
        base.CastAbility(caster, target);
        int healPower = _healPowers[_level];

        GameUnit leastHpAlly= null;
        int leastHp = int.MaxValue;

        // Caster is the only unit on board
        if (caster.Owner.BoardUnits.Count == 1)
        {
            caster.OnHealRecieved(healPower);
            AnitmateAbility(caster);
        }

        // Find the ally with least hp
        else
        {
            foreach (GameUnit ally in caster.Owner.BoardUnits)
            {
                if (ally.Hp < leastHp)
                {
                    leastHp = ally.Hp;
                    leastHpAlly = ally;
                }
            }

            leastHpAlly.OnHealRecieved(healPower);
            AnitmateAbility(leastHpAlly);
        }
    }
}
