using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseMonster : MonoBehaviour
{
    public MonsterData data;
    public int currentHp;

    public MonsterState currentState = MonsterState.IDLE;

    protected SpriteRenderer _sr;
    
    private void Awake()
    {
    	_sr = GetComponent<SpriteRenderer>();
    
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

    private void Start()
    {
        StartCoroutine(IEStateChange());
    }

    private IEnumerator IEStateChange()
    {
        while (true)
        {
            currentState = MonsterState.IDLE;
            yield return StartCoroutine(IEIdle());

            currentState = MonsterState.CHARGE;
            yield return StartCoroutine(IECharging());
            
            currentState = MonsterState.ATTACK;
            yield return StartCoroutine(IEAttackPlayer());
        }

        yield return null;
    }

    protected virtual IEnumerator IEIdle()
    {
        _sr.color = Color.white;
        yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));
    }

    protected virtual IEnumerator IECharging()
    {
        _sr.color = Color.blue;
        yield return new WaitForSeconds(1.5f);
    }

    protected virtual IEnumerator IEAttackPlayer()
    {
        _sr.color = Color.red;
        yield return new WaitForSeconds(1f);
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
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private void ResetMonster()
    {
        currentState = MonsterState.IDLE;
        currentHp = data.hp;
    }
    
    public MonsterType GetMonsterType() => data.type;
    public MonsterSize GetMonsterSize() => data.size;
}

public enum MonsterState
{
    IDLE = 0,
    CHARGE = 1,
    ATTACK = 2,
}
