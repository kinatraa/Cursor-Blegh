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
    protected Animator _animator;
    protected BaseWeapon _currentWeapon;
    private Coroutine _behaviorCoroutine;
    private Coroutine _blinkCoroutine;

    protected const string ANIM_IDLE = "Idle";
    protected const string ANIM_ATTACK = "Attack";
    protected const string ANIM_DIE = "Die";

    protected float _remainingAnimTime;

    public float projectileSpeed = 0f;
    public float reduceAnimTime = 0f;

    [SerializeField] private float _chargePhaseRatio = 0.6f;
    protected float ChargePhaseRatio => _chargePhaseRatio;
    [SerializeField] private bool _flipOnUpdate = true;

    [Header("Visual Effects")] [SerializeField]
    private float _blinkDuration = 0.5f;

    [SerializeField] private int _blinkCount = 3;
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private float _fadeOutDuration = 0.5f;

    public bool isDead = false;

    public bool isFrozen = false;
    private Coroutine _freezeCoroutine;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _currentWeapon = GameplayManager.Instance.weaponController.currentWeapon;

        ResetMonster();
    }

    private void Start()
    {
        _behaviorCoroutine = StartCoroutine(IEStateChange());
    }

    private void Update()
    {
        if (_flipOnUpdate)
        {
            UpdateFacingDirection();
        }
    }

    protected void UpdateFacingDirection()
    {
        if (_currentWeapon)
        {
            Vector3 playerPos = _currentWeapon.transform.position;
            Vector3 directionToPlayer = playerPos - transform.position;

            if (directionToPlayer.x < 0)
            {
                _sr.flipX = true;
            }
            else if (directionToPlayer.x > 0)
            {
                _sr.flipX = false;
            }
        }
    }

    protected void PlayAnimation(string stateName, float transitionDuration = 0.1f)
    {
        if (_animator != null)
        {
            _animator.CrossFade(stateName, transitionDuration);
        }
    }

    private IEnumerator IEStateChange()
    {
        while (true)
        {
            if (isFrozen)
            {
                yield return null;
                continue;
            }

            currentState = MonsterState.IDLE;
            yield return StartCoroutine(IEIdle());

            currentState = MonsterState.CHARGE;
            yield return StartCoroutine(IECharging());

            currentState = MonsterState.ATTACK;
            yield return StartCoroutine(IEAttackPlayer());
        }
    }

    protected virtual IEnumerator IEIdle()
    {
        PlayAnimation(ANIM_IDLE);
        _sr.color = Color.white;
        float randomAnimTime = Random.Range(3.0f, 5.0f);
        float animTime = 0f;
        if (animTime < randomAnimTime - reduceAnimTime) animTime = randomAnimTime - reduceAnimTime;
        yield return new WaitForSeconds(animTime);
    }

    protected virtual IEnumerator IECharging()
    {
        
        string hitKey = "monster_hit";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }
        
        PlayAnimation(ANIM_ATTACK);
        _sr.color = Color.blue;

        yield return null;
        float totalDuration = _animator.GetCurrentAnimatorStateInfo(0).length;

        float chargeTime = totalDuration * _chargePhaseRatio;

        _remainingAnimTime = totalDuration - chargeTime;

        yield return new WaitForSeconds(chargeTime);
    }

    protected virtual IEnumerator IEAttackPlayer()
    {
        _sr.color = Color.red;
        yield return new WaitForSeconds(_remainingAnimTime);
    }

    public virtual void TakeDamage(int damage, bool isPlaySfx = true)
    {
        Debug.Log($"{gameObject.name} is taking damage {damage}");
        
        if (ComboController.Instance != null)
        {
            ComboController.Instance.AddCombo();
        }

        if (isPlaySfx)
        {
            List<string> hitSounds = new List<string> {  "monster_hit1","monster_hit2","monster_hit3","monster_hit4", "sfx_hit1", "sfx_hit2"};
            int randomChance = UnityEngine.Random.Range(0, hitSounds.Count);
            string hitKey = hitSounds[randomChance];
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ShotSfx(hitKey);
            }
        }
        
        currentHp -= damage;

        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }

        _blinkCoroutine = StartCoroutine(IEBlinkEffect());

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator IEBlinkEffect()
    {
        Color originalColor = _sr.color;
        float blinkInterval = _blinkDuration / (_blinkCount * 2);

        for (int i = 0; i < _blinkCount; i++)
        {
            if (isDead) yield break;

            _sr.color = _damageColor;
            yield return new WaitForSeconds(blinkInterval);

            if (isDead) yield break;

            _sr.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }

        if (!isDead)
        {
            _sr.color = originalColor;
        }
    }

    public void Freeze(float duration){
        if (_freezeCoroutine != null){
            StopCoroutine(_freezeCoroutine);
        }
        _freezeCoroutine = StartCoroutine(IEFreeze(duration));
    }

    private IEnumerator IEFreeze(float duration){
        if (!isFrozen && _behaviorCoroutine != null){
            StopCoroutine(_behaviorCoroutine);
            _behaviorCoroutine = null;
        }   

        isFrozen = true;

        currentState = MonsterState.IDLE;
        PlayAnimation(ANIM_IDLE);

        Color originalColor = _sr.color;
        _sr.color = Color.cyan;

        yield return new WaitForSeconds(duration);

        if (!isDead){
            isFrozen = false;
            _sr.color = originalColor;
            if (_behaviorCoroutine == null){
                _behaviorCoroutine = StartCoroutine(IEStateChange());
            }
        }
        _freezeCoroutine = null;
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        if (_behaviorCoroutine != null)
        {
            StopCoroutine(_behaviorCoroutine);
            _behaviorCoroutine = null;
        }

        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }

        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        List<string> hitSounds = new List<string> { "monster_die1",  "monster_die2","monster_die3"};
        int randomChance = UnityEngine.Random.Range(0, hitSounds.Count);
        string hitKey = hitSounds[randomChance];
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }

        StartCoroutine(IEDieSequence());
    }

    protected virtual IEnumerator IEDieSequence()
    {
        PlayAnimation(ANIM_DIE);
        float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        yield return StartCoroutine(IEFadeOut());
        Destroy(gameObject);
    }

    protected IEnumerator IEFadeOut()
    {
        float elapsed = 0f;
        Color startColor = _sr.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _fadeOutDuration;
            _sr.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        _sr.color = targetColor;
    }

    public float GetMonsterRadius()
    {
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            return Mathf.Max(
                spriteRenderer.sprite.bounds.size.x * transform.localScale.x,
                spriteRenderer.sprite.bounds.size.y * transform.localScale.y
            ) * 0.5f;
        }

        return 0.5f;
    }

    private void ResetMonster()
    {
        currentState = MonsterState.IDLE;
        currentHp = data.hp;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (GameplayManager.Instance != null &&
            GameplayManager.Instance.waveController != null)
        {
            bool shouldDropHealth = ShouldDropHealthPack();
            if (shouldDropHealth)
            {
                DropHealthPack();
            }
            
            if (GameplayManager.Instance.monsterController.lastHitMonster == this)
            {
                GameplayManager.Instance.monsterController.lastHitMonster = null;
            }
            
            int baseScore = data.score;
            int comboBonus = 0;
            
            if (ComboController.Instance != null)
            {
                comboBonus = ComboController.Instance.CalculateComboBonus();
            }
            
            int totalScore = baseScore + comboBonus;
            
            GameplayManager.Instance.weaponController.currentWeapon.GainScore(totalScore);
            
            Debug.Log($"<color=yellow>Score: {baseScore} + {comboBonus} (Combo {ComboController.Instance?.GetCurrentCombo()}x) = {totalScore}</color>");
            
            GameplayManager.Instance.monsterController.RemoveMonster(this);
        }
    }

    private bool ShouldDropHealthPack()
    {
        float baseDropChance = 0.05f;
        float totalDropChance = baseDropChance;
        
        var healingBuff = GameplayManager.Instance.monsterController.healingHerbBuff;
        if (healingBuff != null)
        {
            float buffBonus = healingBuff.GetStack * 0.05f;
            totalDropChance += buffBonus;
            
            Debug.Log($"<color=cyan>Health drop chance: {baseDropChance * 100}% + {buffBonus * 100}% (Buff {healingBuff.GetStack}x) = {totalDropChance * 100}%</color>");
        }
        
        return Random.value < totalDropChance;
    }

    private void DropHealthPack()
    {
        var healingHerbData = GameplayManager.Instance.buffController.buffSystem.GetBuffData(BuffType.HEALING_HERB);
        if (healingHerbData == null) return;

        GameObject healthPrefab = healingHerbData.prefab;
    
        if (healthPrefab != null)
        {
            Instantiate(healthPrefab, transform.position, Quaternion.identity);
            Debug.Log($"<color=green>❤ Health pack dropped!</color>");
        }
        else
        {
            Debug.LogWarning("Health pack prefab not found");
        }
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