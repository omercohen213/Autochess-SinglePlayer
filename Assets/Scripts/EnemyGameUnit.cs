using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGameUnit : GameUnit
{
    public void DropItem()
    {
        ItemDropManager.CreateItemOrb(transform.position);
    }
}
