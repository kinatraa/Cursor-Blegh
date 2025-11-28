using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfernoBeast : BaseMonster
{

    [Header("Attack Settings")]
    [SerializeField] private BulletInferno bulletPrefab;
    [SerializeField] private int minBullets = 1;
    [SerializeField] private int maxBullets = 2;
    [SerializeField] private float bulletSpawnRadius = 0.5f;

    protected override IEnumerator IEAttackPlayer()
    {
        _sr.color = Color.red;

        int bulletCount = Random.Range(minBullets, maxBullets + 1);
        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBullet();
        }

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
