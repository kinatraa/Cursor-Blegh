using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelShieldObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ConstTag.MONSTER_PROJECTILE))
        {
            var projectile = other.transform.GetComponent<BaseMonsterProjectile>();
            projectile.Destroy();
        }
    }
}
