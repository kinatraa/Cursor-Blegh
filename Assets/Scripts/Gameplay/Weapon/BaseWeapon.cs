using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public WeaponData data;

    public int currentHp;
    public int currentScore;
    
    public WeaponState currentState = WeaponState.NORMAL;
    
    private SpriteRenderer _sr;
    private Coroutine _takeDamageCoroutine;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState == WeaponState.SKILL_ACTIVE && data.weaponType == WeaponType.WOODEN_SWORD) return;

        if (currentState == WeaponState.SKILL_ACTIVE && data.weaponType == WeaponType.BATTLE_AXE) return;

        if (other.CompareTag(ConstTag.MONSTER))
        {
            BaseMonster monster = other.GetComponent<BaseMonster>();
            if (currentState == WeaponState.SKILL_ACTIVE && data.weaponType == WeaponType.DAGGER){
                CombatResolver.WeaponDamageToMonster(this, monster);
            } else {
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
            _takeDamageCoroutine = StartCoroutine(IETakeDamageAlert());
        }

        currentHp -= damage;
        GameEventManager.InvokeUpdatePlayerHp(currentHp);
        if (currentHp <= 0)
        {
            // Lose
            Destroy(gameObject);
        }
    }

    private IEnumerator IETakeDamageAlert()
    {
        currentState = WeaponState.BLINK;
        int times = 3;
        while (times-- > 0)
        {
            _sr.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _sr.color = Color.white;
            yield return new WaitForSeconds(0.2f);
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
        currentHp = data.hp;
        currentScore = 0;
        GameEventManager.InvokeUpdatePlayerMaxHp(currentHp);
        GameEventManager.InvokeUpdatePlayerScore(currentScore);
    }
}

public enum WeaponState
{
    NORMAL = 0,
    BLINK = 1,
    SKILL_ACTIVE = 2
}
