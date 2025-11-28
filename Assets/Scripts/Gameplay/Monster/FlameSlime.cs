using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSlime : BaseMonster
{
    [Header("Attack Settings")]
    [SerializeField] private BaseMonsterProjectile bulletPrefab;
    [SerializeField] private float bulletSpawnRadius = 0.5f;
    [SerializeField] private float shootDistance = 10f;
    
    protected override IEnumerator IEAttackPlayer()
    {
        _sr.color = Color.red;
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            SpawnBulletInDirection(angle);
        }

        yield return new WaitForSeconds(_remainingAnimTime);
        _sr.color = Color.white;
    }

    private void SpawnBulletInDirection(float angleDegrees)
    {
        if (bulletPrefab == null){
            Debug.LogWarning("Bullet prefab is not assigned!");
            return;
        }

        Vector3 spawnPosition = transform.position;

        var bulletInstance = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians), 0);
        Vector3 targetPosition = transform.position + direction * shootDistance;

        bulletInstance.StartProjectile(targetPosition);

        Debug.Log($"FlameSlime shot a bullet towards angle {angleDegrees} degrees.");
    }
}
