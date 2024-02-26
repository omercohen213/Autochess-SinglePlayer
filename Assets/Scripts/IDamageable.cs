using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void OnDamageRecieved(GameUnit attacker, GameUnit target, int damageAmount);
    //void GetStun(float duration);
    void Death();
}

