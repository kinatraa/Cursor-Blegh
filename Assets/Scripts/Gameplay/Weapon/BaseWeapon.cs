using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public WeaponData data;

    public int currentHp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ConstTag.MONSTER))
        {
            BaseMonster monster = other.GetComponent<BaseMonster>();
            CombatResolver.CollisionResolve(this, monster);
        }
    }

    public void TakeDamage(int damage = 1)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            // Lose
            Destroy(gameObject);
        }
    }
    
    public void ResetWeapon()
    {
        currentHp = data.hp;
    }
}
