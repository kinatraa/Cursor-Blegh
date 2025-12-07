using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class DemonKing : BaseMonster
{
     private const string ANIM_SKILL2 = "Skill2";
     private const string ANIM_SKILL3 = "Skill3";
     private const string ANIM_SKILL4 = "Skill4";
     private const string ANIM_SKILL4_START = "StartSkill4";
     
     [Header("Skill settings")]
     [SerializeField] private BaseMonsterProjectile _bulletPrefab;
      private const float BulletSpawnRadius = 0.5f;
      private const float ShootDistance = 0.5f;
      private const float ExploreOffsetDuration = 0.25f;
      private const float ExploreFieldDuration = 1.0f;
      private const float FieldRadiusMultiplier = 2f;
      private const float DamageInterval = 0.01f;
      
      private const float FadeOutDurationTeleport = 0.5f; 
      private const float InvisibleDuration = 2f;
      private const float FadeInDuration = 0.5f;
      
      private const float NormalSpeed = 5f;
      private const float FastSpeed = 15f;
      
      [SerializeField] private LaserBeam _laserPrefab;
      private const int LaserCount = 4;
      private const float LaserLength = 30f;
      private bool _randomAngles = true; 

     private bool _isExplore = false;
     private bool _isInvincible = false;
     private float _currentFieldRadius = 0f;
     private HashSet<BaseWeapon> _damagedWeaponsThisInterval = new HashSet<BaseWeapon>();

     protected override IEnumerator IECharging()
     {
          int attackState = Random.Range(0, 4);
          // _sr.color = Color.blue;

          switch (attackState)
          {
               case 0:
                    PlayAnimation(ANIM_ATTACK);
                    break;
               case 1:
                    PlayAnimation(ANIM_SKILL2);
                    break;
               case 2:
                    PlayAnimation(ANIM_SKILL3);
                    break;
               default:
                    PlayAnimation(ANIM_SKILL4);
                    break;
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
               yield return StartCoroutine(IESkill1());
          }
          else if (currentAnimState.IsName(ANIM_SKILL2))
          {
               yield return StartCoroutine(IESkill2());
          } else if (currentAnimState.IsName(ANIM_SKILL3))
          {
               yield return StartCoroutine(IESkill3());
          } else if (currentAnimState.IsName(ANIM_SKILL4))
          {
               yield return StartCoroutine(IESkill4());
          }
          _sr.color = Color.white;
     }

     private IEnumerator IESkill1()
     {
          // _sr.color = Color.yellow;
          ShootLasers();
          
          yield return new WaitForSeconds(_remainingAnimTime);
          _sr.color = Color.white;
     }
     
     private void ShootLasers()
     {
          if (_laserPrefab == null)
          {
               Debug.LogWarning("LaserBeam prefab is not assigned!");
               return;
          }
          
          string hitKey = "monster_lasershoot";
          if (AudioManager.Instance != null)
          {
               AudioManager.Instance.ShotSfx(hitKey);
          }
          
          for (int i = 0; i < LaserCount; i++)
          {
               float randomAngle = Random.Range(0f, 360f);
               SpawnLaser(randomAngle);
          }
        
          Debug.Log($"<color=cyan>Demon King fired {LaserCount} lasers!</color>");
     }
     
     private void SpawnLaser(float angleDegrees)
     {
          var laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        
          float angleRadians = angleDegrees * Mathf.Deg2Rad;
          Vector3 direction = new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians), 0f);
        
          laser.Initialize(transform.position, direction, LaserLength);
        
          Debug.Log($"<color=yellow>Laser spawned at angle {angleDegrees}°</color>");
     }
     
     private IEnumerator IESkill2()
     {
          // _sr.color = Color.red;
          string hitKey = "monster_shoot";
          if (AudioManager.Instance != null)
          {
               AudioManager.Instance.ShotSfx(hitKey);
          }
          for (int i = 0; i < 12; i++)
          {
               float angle = i * 30f;
               SpawnBulletInDirection(angle, NormalSpeed);
          }
          
          yield return new WaitForSeconds(_remainingAnimTime);
          _sr.color = Color.white;
     }
     
     private IEnumerator IESkill3()
     {
          // _sr.color = Color.red;
          string hitKey = "monster_shoot";
          if (AudioManager.Instance != null)
          {
               AudioManager.Instance.ShotSfx(hitKey);
          }
          SpawnBullet(FastSpeed);
          yield return new WaitForSeconds(_remainingAnimTime);
          _sr.color = Color.white;
     }
     
     private IEnumerator IESkill4()
     {
          // _sr.color = Color.red;
          _isInvincible = true;
          
          string hitKey = "monster_disappear";
          if (AudioManager.Instance != null)
          {
               AudioManager.Instance.ShotSfx(hitKey);
          }
          
          yield return StartCoroutine(IETeleportToPlayer());

          yield return new WaitForSeconds(ExploreOffsetDuration);
          
          PlayAnimation(ANIM_SKILL4_START);
          
          hitKey = "monster_appear";
          if (AudioManager.Instance != null)
          {
               AudioManager.Instance.ShotSfx(hitKey);
          }
          
          hitKey = "monster_explode";
          if (AudioManager.Instance != null)
          {
               AudioManager.Instance.ShotSfx(hitKey);
          }
          
          yield return StartCoroutine(Explore());
          
          yield return new WaitForSeconds(_remainingAnimTime);
          _isInvincible = false;
          _sr.color = Color.white;
     }

     private IEnumerator IETeleportToPlayer()
     {
          var playerPosition = GameplayManager.Instance.weaponController.currentWeapon.transform.position;
          if (playerPosition == Vector3.zero)
          {
               Debug.LogWarning("Teleport player position is not set!");
               yield break;
          }
          
          yield return StartCoroutine(IEFadeOutTeleport());

          transform.position = playerPosition;

          yield return new WaitForSeconds(InvisibleDuration);

          yield return StartCoroutine(IEFadeInTeleport());
     }
     
     private IEnumerator IEFadeOutTeleport()
     {
          float elapsed = 0f;
          Color startColor = _sr.color;
          Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
          while (elapsed < FadeOutDurationTeleport)
          {
               elapsed += Time.deltaTime;
               float t = elapsed / FadeOutDurationTeleport;
               _sr.color = Color.Lerp(startColor, targetColor, t);
               yield return null;
          }
        
          _sr.color = targetColor;
     }
    
     private IEnumerator IEFadeInTeleport()
     {
          float elapsed = 0f;
          Color startColor = _sr.color;
          Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
        
          while (elapsed < FadeInDuration)
          {
               elapsed += Time.deltaTime;
               float t = elapsed / FadeInDuration;
               _sr.color = Color.Lerp(startColor, targetColor, t);
               yield return null;
          }
        
          _sr.color = targetColor;
     }

     public override void TakeDamage(int damage, bool isPlaySfx = true){
          if (_isInvincible){
               Debug.Log("Demon King is invincible and took no damage.");
               return;
          }
          base.TakeDamage(damage);
     }

     private void SpawnBullet(float bulletSpeed){
          if (_bulletPrefab == null){
               Debug.LogWarning("Bullet prefab is not assigned!");
               return;
          }

          Vector3 spawnOffset = Random.insideUnitCircle.normalized * BulletSpawnRadius;
          Vector3 spawnPosition = transform.position + spawnOffset;
        
          var bulletInstance = Instantiate(_bulletPrefab, spawnPosition, Quaternion.identity);

          bulletInstance.speed = bulletSpeed;
          bulletInstance.speed *= (int)(1 + projectileSpeed/100);

          Vector3 playerPosition = GameplayManager.Instance.weaponController.currentWeapon.transform.position;

          bulletInstance.StartProjectile(playerPosition);

          Debug.Log("Demon King spawned a bullet towards the player.");
     }
     
     private IEnumerator Explore()
     {
          float monsterRadius = GetMonsterRadius();
          _currentFieldRadius = monsterRadius * FieldRadiusMultiplier;
          _isExplore = true;
          
          Debug.Log("Demon King kaboom");
          
          
          float elapsed = 0f;
          float lastDamageTime = 0f;
          while (elapsed < ExploreFieldDuration)
          {
               elapsed += Time.deltaTime;
            
               // Deal damage
               if (elapsed - lastDamageTime >= DamageInterval)
               {
                    DealDamageInField();
                    lastDamageTime = elapsed;
                    _damagedWeaponsThisInterval.Clear();
               }
            
               yield return null;
          }
          DeactiveExploration();

     }
     
     private void DealDamageInField()
     {
        
          var weaponController = GameplayManager.Instance.weaponController;
          if (weaponController == null) return;
        
          var currentWeapon = weaponController.currentWeapon;
          if (currentWeapon == null) return;
        
          float distance = Vector3.Distance(transform.position, currentWeapon.transform.position);
        
          if (distance <= _currentFieldRadius)
          {
               if (!_damagedWeaponsThisInterval.Contains(currentWeapon))
               {
                    CombatResolver.MonsterDamageWeapon(currentWeapon, this);
                    _damagedWeaponsThisInterval.Add(currentWeapon);
                
                    Debug.Log($"<color=yellow>ThunderBeast zapped {currentWeapon.name} (distance: {distance:F2})</color>");
               }
          }
     }
     
     private void DeactiveExploration()
     {
          _isExplore = false;
          _currentFieldRadius = 0f;
          _damagedWeaponsThisInterval.Clear();
        
          Debug.Log("<color=cyan>ThunderBeast deactivated electric field</color>");
     }
     
     private void SpawnBulletInDirection(float angleDegrees, float bulletSpeed)
     {
          if (_bulletPrefab == null){
               Debug.LogWarning("Bullet prefab is not assigned!");
               return;
          }

          Vector3 spawnPosition = transform.position;

          var bulletInstance = Instantiate(_bulletPrefab, spawnPosition, Quaternion.identity);
          bulletInstance.speed = bulletSpeed;

          float angleRadians = angleDegrees * Mathf.Deg2Rad;
          Vector3 direction = new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians), 0);
          Vector3 targetPosition = transform.position + direction * ShootDistance;

          bulletInstance.StartProjectile(targetPosition);

          Debug.Log($"Demon King shot a bullet towards angle {angleDegrees} degrees.");
     }
}
