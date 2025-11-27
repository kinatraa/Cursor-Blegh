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
        if (data != null)
        {
            var clonedData = ScriptableObject.CreateInstance<MonsterData>();
            clonedData.monsterName = data.monsterName;
            clonedData.description = data.description;
            clonedData.type = data.type;
            clonedData.size = data.size;
            clonedData.rank = data.rank;
            clonedData.hp = data.hp;
            clonedData.score = data.score;
            clonedData.attackType = data.attackType;
            data = clonedData;
        }
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
    
    public MonsterType GetMonsterType() => data.type;
    public MonsterSize GetMonsterSize() => data.size;
}