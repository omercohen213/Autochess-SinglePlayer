using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void OnDamageTaken(int damage);
    //void GetStun(float duration);
    void Die();
}

