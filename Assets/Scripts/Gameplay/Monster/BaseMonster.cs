using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMonster : MonoBehaviour
{
    public MonsterData data;
    public int currentHp;

    public bool onAttack = false;
    
    private void Awake()
    {
        ResetMonster();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} is taking damage {damage}");
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} dead");
        Destroy(gameObject);
    }

    private void ResetMonster()
    {
        onAttack = false;
        currentHp = data.hp;
    }
}
