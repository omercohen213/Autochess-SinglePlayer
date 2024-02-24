using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void ReceiveDamage(int damageAmount);
    //void GetStun(float duration);
    bool IsDamageToKill(float damage);
    void Death();
}

