using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrakeBeast : BaseMonster
{
    private const string ANIM_SKILL = "Skill";
    private const string ANIM_START_ATTACK = "StartAttack";
    
    [Header("Bullet Settings")]
    [SerializeField] private BaseMonsterProjectile _bulletPrefab;
    [SerializeField] private float _bulletSpawnRadius = 0.5f;
    [SerializeField] private float _breathDuration = 3f; 
    [SerializeField] private float _minBreathDuration = 2f;
    [SerializeField] private float _maxBreathDuration = 4f;
    [SerializeField] private float _bulletSpawnInterval = 0.15f;
    [SerializeField] private float _bulletSpawnDistance = 1f;

    [Header("Attack Settings")]
    [SerializeField] private float _lungeSpeed = 8f; 
    [SerializeField] private float _lungeDuration = 0.5f;
    [SerializeField] private float _damageRadius = 1f;
    [SerializeField] private float _lungeStartRatio = 0.5f;


    private bool _isBreathing = false;


    protected override IEnumerator IECharging()
    {
        int attackState = Random.Range(0, 2);
        _sr.color = Color.blue;
        if (attackState == 1)
        {
            PlayAnimation(ANIM_ATTACK);
        }
        else
        {
            PlayAnimation(ANIM_SKILL);
        }
        
        yield return null;
        
        float totalDuration = _animator.GetCurrentAnimatorStateInfo(0).length;
        
        float chargeTime = totalDuration * ChargePhaseRatio;
        
        _remainingAnimTime = totalDuration - chargeTime;
        
        yield return new WaitForSeconds(chargeTime);
    }

    protected override IEnumerator IEAttackPlayer()
    {
        var currentAnimState = _animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimState.IsName(ANIM_ATTACK))
        {
            yield return StartCoroutine(IELungeAttack());
        }
        else if (currentAnimState.IsName(ANIM_SKILL))
        {
            yield return StartCoroutine(IEBreathAttack());
        }
        _sr.color = Color.white;
    }

    private IEnumerator IELungeAttack(){
        var weaponController = GameplayManager.Instance.weaponController;
        if (weaponController == null || weaponController.currentWeapon == null){
            yield return new WaitForSeconds(_remainingAnimTime);
            yield break;
        }
        
        _sr.color = Color.red;
        
        yield return new WaitForSeconds(_remainingAnimTime);

        var currentWeapon = weaponController.currentWeapon;
        Vector3 startPos = transform.position;
        Vector3 targetPos = currentWeapon.transform.position;

        float elapsed = 0f;
        bool hasDamaged = false;

        while (elapsed < _lungeDuration){
            elapsed += Time.deltaTime;
            float t = elapsed / _lungeDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            PlayAnimation(ANIM_START_ATTACK);

            if (!hasDamaged){
                float distanceToWeapon = Vector3.Distance(transform.position, currentWeapon.transform.position);
                if (distanceToWeapon <= _damageRadius){
                    CombatResolver.MonsterDamageWeapon(currentWeapon, this);
                    Debug.Log("DrakeBeast damaged the player's weapon during lunge attack.");
                    hasDamaged = true;
                }

            }
            
            yield return null;
        }
        float remainingTime = _remainingAnimTime - _lungeDuration;
        if (remainingTime > 0){
            yield return new WaitForSeconds(remainingTime);
        }
    }

    private IEnumerator IEBreathAttack(){
        if (_bulletPrefab == null){
            Debug.LogWarning("Bullet prefab is not assigned!");
            yield return new WaitForSeconds(_remainingAnimTime);
            yield break;
        }

        _sr.color = new Color(1f, 0.5f, 0f);
        _isBreathing = true;

        float actualBreathDuration = Random.Range(_minBreathDuration, _maxBreathDuration);

        float elapsed = 0f;
        float lastSpawnTime = 0f;

        var weaponController = GameplayManager.Instance.weaponController;
        
        while (elapsed < actualBreathDuration && _isBreathing){
            elapsed += Time.deltaTime;

            if (elapsed - lastSpawnTime >= _bulletSpawnInterval){
                SpawnBreathBullet(weaponController);
                lastSpawnTime = elapsed;
            }

            yield return null;
        }

        _isBreathing = false;

        float remainingTime = _remainingAnimTime - actualBreathDuration;
        if (remainingTime > 0){
            yield return new WaitForSeconds(remainingTime);
        }
    }

    private void SpawnBreathBullet(WeaponController weaponController){
        if (weaponController == null || weaponController.currentWeapon == null){
            return;
        }

        Vector3 playerPosition = weaponController.currentWeapon.transform.position;
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;

        var bulletInstance = Instantiate(_bulletPrefab, transform.position + directionToPlayer * _bulletSpawnDistance, Quaternion.identity);
        bulletInstance.speed *= (int)(1 + projectileSpeed/100);
        bulletInstance.StartProjectile(playerPosition);
    }

    protected override void Die(){
        _isBreathing = false;
        base.Die();
    }
}
