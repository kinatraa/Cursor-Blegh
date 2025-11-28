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
    private Coroutine _behaviorCoroutine;
    
    protected const string ANIM_IDLE = "Idle";
    protected const string ANIM_ATTACK = "Attack";
    protected const string ANIM_DIE = "Die";
    
    protected float _remainingAnimTime;
    
    [SerializeField] private float _chargePhaseRatio = 0.6f;
    [SerializeField] private bool _flipOnUpdate= true;
    
    private void Awake()
    {
    	_sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        ResetMonster();
    }

    private void Start()
    {
        _behaviorCoroutine = StartCoroutine(IEStateChange());
    }

    private void Update(){
        if (_flipOnUpdate){
            UpdateFacingDirection();
        }
    }

    protected void UpdateFacingDirection(){
        var playerController = GameplayManager.Instance.weaponController.currentWeapon;
        if (playerController != null)
        {
            Vector3 playerPos = playerController.transform.position;
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
        yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));
    }

    protected virtual IEnumerator IECharging()
    {
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
        if (_behaviorCoroutine != null) StopCoroutine(_behaviorCoroutine);
        StartCoroutine(IEDieSequence());
    }
    
    protected virtual IEnumerator IEDieSequence()
    {
        PlayAnimation(ANIM_DIE);
        yield return null;
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
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
    
    public MonsterType GetMonsterType() => data.type;
    public MonsterSize GetMonsterSize() => data.size;
}

public enum MonsterState
{
    IDLE = 0,
    CHARGE = 1,
    ATTACK = 2,
}
