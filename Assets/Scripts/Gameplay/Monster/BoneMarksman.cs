using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMarksman : BaseMonster
{
    [Header("Attack Settings")]
    [SerializeField] private BaseMonsterProjectile bulletPrefab;
    [SerializeField] private float bulletSpawnRadius = 0.5f;
    
    protected override IEnumerator IEAttackPlayer()
    {
        _sr.color = Color.red;
        SpawnBullet();

        yield return new WaitForSeconds(_remainingAnimTime);
    }

    private void SpawnBullet(){
        if (bulletPrefab == null){
            Debug.LogWarning("Bullet prefab is not assigned!");
            return;
        }

        Vector3 spawnOffset = Random.insideUnitCircle.normalized * bulletSpawnRadius;
        Vector3 spawnPosition = transform.position + spawnOffset;
        
        var bulletInstance = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        Vector3 playerPosition = GameplayManager.Instance.weaponController.currentWeapon.transform.position;

        bulletInstance.StartProjectile(playerPosition);

        Debug.Log("InfernoBeast spawned a bullet towards the player.");
    }
}
