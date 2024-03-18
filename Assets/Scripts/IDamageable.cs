using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void OnDamageTaken(int damage, bool isMagic, bool isCritical);
    //void GetStun(float duration);
    void Die();
}

