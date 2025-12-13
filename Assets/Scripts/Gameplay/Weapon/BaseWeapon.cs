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
    public bool isImmortal = false;

    [Header("Slash Effect")]
    [SerializeField] private SlashEffect _slashEffect;

    public SlashEffect GetSlashEffect()
    {
        if (_slashEffect != null) return _slashEffect;
        _slashEffect = GetComponentInChildren<SlashEffect>(true);
        return _slashEffect;
    }

    private SpriteRenderer _sr;
    private Coroutine _takeDamageCoroutine;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState == WeaponState.SKILL_ACTIVE && 
            (data.weaponType == WeaponType.WOODEN_SWORD || data.weaponType == WeaponType.BATTLE_AXE))
            return;

        if (other.CompareTag(ConstTag.MONSTER))
        {
            BaseMonster monster = other.GetComponent<BaseMonster>();
            if (monster == null) return;

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
            if (projectile == null) return;

            CombatResolver.CollisionResolve(this, projectile);
        }
    }

    // Flash red
    private IEnumerator IETakeDamageAlert()
    {
        WeaponState previousState = currentState;
        currentState = WeaponState.BLINK;

        int times = 3;
        while (times-- > 0)
        {
            _sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            _sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        if (previousState == WeaponState.NORMAL)
            currentState = WeaponState.NORMAL;

        _takeDamageCoroutine = null;
    }

    public void TakeDamage(int damage = 1)
    {
        if (isImmortal) return;

        if (currentState == WeaponState.NORMAL)
        {
            if (_takeDamageCoroutine != null)
                StopCoroutine(_takeDamageCoroutine);
            _takeDamageCoroutine = StartCoroutine(IETakeDamageAlert());
        }

        List<string> hitSounds = new List<string> { "weapon_hit1", "weapon_hit2" };
        int randomChance = UnityEngine.Random.Range(0, hitSounds.Count);
        AudioManager.Instance?.ShotSfx(hitSounds[randomChance]);

        currentHp -= damage;
        GameEventManager.InvokeUpdatePlayerHp(currentHp);

        GameplayManager.Instance.waveRewardSystem.OnDamageTaken();

        if (currentHp <= 0)
        {
            var rebornBuff = GameplayManager.Instance.weaponController.rebornBuff;
            if (rebornBuff != null && rebornBuff.TryRevive())
            {
                AudioManager.Instance?.ShotSfx("item_pickup");

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

    public void HealByItem(int amount)
    {
        int totalHeal = amount;

        AudioManager.Instance?.ShotSfx("item_pickup");

        if (GameplayManager.Instance.monsterController.mysticPotionBuff != null)
            totalHeal += 1;

        currentHp = Mathf.Min(currentHp + totalHeal, maxHp);
        GameEventManager.InvokeUpdatePlayerHp(currentHp);
    }

    public void GainScore(int score)
    {
        currentScore += score;
        GameEventManager.InvokeUpdatePlayerScore(currentScore);
    }

    public void ResetWeapon()
    {
        isImmortal = false;
        maxHp = data.hp;
        currentHp = maxHp;
        critDmgToAdd = 0;
        critChanceToAdd = 0f;
        damageToAdd = 0;
        currentScore = 0;

        currentState = WeaponState.NORMAL;

        GameEventManager.InvokeUpdatePlayerMaxHp(maxHp);
        GameEventManager.InvokeUpdatePlayerHp(currentHp);
        GameEventManager.InvokeUpdatePlayerScore(currentScore);

        SlashEffect slash = GetSlashEffect();
        if (slash != null)
            slash.gameObject.SetActive(false);
    }

    // -----------------------------------------------------------
    // PLAY SLASH (CHỈ BẬT - TỰ TẮT BẰNG ANIMATION EVENT)
    // -----------------------------------------------------------
    public void PlaySlash(bool flip = false)
    {
        SlashEffect slash = GetSlashEffect();
        if (slash == null) return;

        // Flip theo nhu cầu
        slash.transform.localScale = new Vector3(
            slash.transform.localScale.x,
            flip ? -Mathf.Abs(slash.transform.localScale.y) : Mathf.Abs(slash.transform.localScale.y),
            slash.transform.localScale.z
        );

        // Reset để animation chạy lại
        slash.gameObject.SetActive(false);
        slash.gameObject.SetActive(true);
    }
}

public enum WeaponState
{
    NORMAL = 0,
    BLINK = 1,
    SKILL_ACTIVE = 2
}
