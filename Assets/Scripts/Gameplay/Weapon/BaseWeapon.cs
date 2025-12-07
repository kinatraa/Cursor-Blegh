using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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
    public bool isImmortal = false;
    
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
        if (isImmortal) return;
        
        if (currentState == WeaponState.NORMAL)
        {
            if (_takeDamageCoroutine != null)
            {
                StopCoroutine(_takeDamageCoroutine);
            }
            _takeDamageCoroutine = StartCoroutine(IETakeDamageAlert());
        }

        List<string> hitSounds = new List<string> { "weapon_hit1", "weapon_hit2" };
        int randomChance = UnityEngine.Random.Range(0, hitSounds.Count);
        string hitKey = hitSounds[randomChance];
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }
        
        currentHp -= damage;
        GameEventManager.InvokeUpdatePlayerHp(currentHp);

        GameplayManager.Instance.waveRewardSystem.OnDamageTaken();
        
        if (currentHp <= 0)
        {
            var rebornBuff = GameplayManager.Instance.weaponController.rebornBuff;
            if (rebornBuff != null && rebornBuff.TryRevive())
            {
                string hitKey1 = "item_pickup";
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ShotSfx(hitKey1);
                }
                currentHp = 1;
                GameEventManager.InvokeUpdatePlayerMaxHp(maxHp);
                GameEventManager.InvokeUpdatePlayerHp(currentHp);

                StartCoroutine(IEReviveEffect());
                
                rebornBuff.Remove();
            }
            else
            {
                GameEventManager.InvokeGameLose();
                GameplayManager.Instance.weaponController.SetDefaultCursor();
            }
        }
    }
    
    private IEnumerator IEReviveEffect()
    {
        int times = 5;
        while (times-- > 0)
        {
            _sr.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
            _sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void HealByItem(int amount){
        int totalHeal = amount;
        string hitKey = "item_pickup";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }
        if (GameplayManager.Instance.monsterController.mysticPotionBuff != null)
        {
            totalHeal += 1;
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
        isImmortal = false;
        if(data != null) maxHp = data.hp;
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