using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMarksman : BaseMonster
{
    [Header("Attack Settings")]
    [SerializeField] private BaseMonsterProjectile _bulletPrefab;
    [SerializeField] private float _bulletSpawnRadius = 0.5f;
    
    protected override IEnumerator IEAttackPlayer()
    {
        // _sr.color = Color.red;
        
        string hitKey = "monster_shoot";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }
        
        SpawnBullet();

        yield return new WaitForSeconds(_remainingAnimTime);
    }

    private void SpawnBullet(){
        if (_bulletPrefab == null){
            Debug.LogWarning("Bullet prefab is not assigned!");
            return;
        }

        Vector3 spawnOffset = Random.insideUnitCircle.normalized * _bulletSpawnRadius;
        Vector3 spawnPosition = transform.position + spawnOffset;
        
        var bulletInstance = Instantiate(_bulletPrefab, spawnPosition, Quaternion.identity);
        bulletInstance.speed *= (int)(1 + projectileSpeed/100);

        Vector3 playerPosition = GameplayManager.Instance.weaponController.currentWeapon.transform.position;

        bulletInstance.StartProjectile(playerPosition);

        Debug.Log("InfernoBeast spawned a bullet towards the player.");
    }
}
