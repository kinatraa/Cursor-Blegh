using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public WeaponData data;

    public int maxHp;
    public int currentHp;
    public int currentScore;
    public float critChanceToAdd = 0f;
    public float critDmgToAdd = 0f;
    public int damageToAdd = 0;
    
    public WeaponState currentState = WeaponState.NORMAL;
    
    private SpriteRenderer _sr;
    private Coroutine _takeDamageCoroutine;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (currentState == WeaponState.SKILL_ACTIVE && data.weaponType == WeaponType.WOODEN_SWORD) 
            return;

        if (currentState == WeaponState.SKILL_ACTIVE && data.weaponType == WeaponType.BATTLE_AXE) 
            return;

        if (other.CompareTag(ConstTag.MONSTER))
        {
            BaseMonster monster = other.GetComponent<BaseMonster>();
            GameplayManager.Instance.monsterController.lastHitMonster = monster;
            
            if (currentState == WeaponState.SKILL_ACTIVE && data.weaponType == WeaponType.DAGGER)
            {
                CombatResolver.WeaponDamageToMonster(this, monster);
            }
            else
            {
                CombatResolver.CollisionResolve(this, monster);
            }
            GameplayManager.Instance.buffController.CheckBuffs();
        }
        else if (other.CompareTag(ConstTag.MONSTER_PROJECTILE))
        {
            BaseMonsterProjectile projectile = other.GetComponent<BaseMonsterProjectile>();
            CombatResolver.CollisionResolve(this, projectile);
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (currentState == WeaponState.NORMAL)
        {
            if (_takeDamageCoroutine != null)
            {
                StopCoroutine(_takeDamageCoroutine);
            }
            _takeDamageCoroutine = StartCoroutine(IETakeDamageAlert());
        }

        currentHp -= damage;
        GameEventManager.InvokeUpdatePlayerHp(currentHp);
        
        if (currentHp <= 0)
        {
            Debug.Log("<color=red>WEAPON DESTROYED!</color>");
            Destroy(gameObject);
        }
    }

    public void HealByItem(int amount){
        int totalHeal = amount;
        if (GameplayManager.Instance.monsterController.mysticPotionBuff != null)
        {
            totalHeal += 2;
        }
        currentHp = Mathf.Min(currentHp + totalHeal, maxHp);
        GameEventManager.InvokeUpdatePlayerHp(currentHp);
    }

    private IEnumerator IETakeDamageAlert()
    {
        WeaponState previousState = currentState;
        currentState = WeaponState.BLINK;
        
        int times = 3;
        while (times-- > 0)
        {
            _sr.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _sr.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }

        if (previousState == WeaponState.NORMAL)
        {
            currentState = WeaponState.NORMAL;
        }
        
        _takeDamageCoroutine = null;
    }

    public void GainScore(int score)
    {
        Debug.Log("Score: " + score);
        currentScore += score;
        GameEventManager.InvokeUpdatePlayerScore(currentScore);
    }
    
    public void ResetWeapon()
    {
        maxHp = data.hp;
        currentHp = maxHp;
        critDmgToAdd = 0;
        critChanceToAdd = 0f;
        currentScore = 0;
        currentState = WeaponState.NORMAL;
        GameEventManager.InvokeUpdatePlayerMaxHp(maxHp);
        GameEventManager.InvokeUpdatePlayerHp(currentHp);
        GameEventManager.InvokeUpdatePlayerScore(currentScore);
    }
}

public enum WeaponState
{
    NORMAL = 0,
    BLINK = 1,
    SKILL_ACTIVE = 2
}