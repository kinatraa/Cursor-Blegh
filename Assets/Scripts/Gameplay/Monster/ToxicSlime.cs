using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicSlime : BaseMonster
{
    [Header("Attack Settings")]
    [SerializeField] private BaseMonsterProjectile _bulletPrefab;
    [SerializeField] private float _bulletSpawnRadius = 0.5f;
    [SerializeField] private float _shootDistance = 10f;
    protected override IEnumerator IEAttackPlayer()
    {
        Debug.Log("This monster does not attack");
        yield return null;
    }

    protected override IEnumerator IECharging()
    {
        yield return null;
    }
    
    protected override IEnumerator IEDieSequence()
    {
        PlayAnimation(ANIM_DIE);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return StartCoroutine(IEFadeOut());
        string hitKey = "monster_shoot";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            SpawnBulletInDirection(angle);
        }
        Destroy(gameObject);
    }
    
    private void SpawnBulletInDirection(float angleDegrees)
    {
        if (_bulletPrefab == null){
            Debug.LogWarning("Bullet prefab is not assigned!");
            return;
        }

        Vector3 spawnPosition = transform.position;

        var bulletInstance = Instantiate(_bulletPrefab, spawnPosition, Quaternion.identity);
        bulletInstance.speed *= (int)(1 + projectileSpeed/100);

        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians), 0);
        Vector3 targetPosition = transform.position + direction * _shootDistance;

        bulletInstance.StartProjectile(targetPosition);

        Debug.Log($"FlameSlime shot a bullet towards angle {angleDegrees} degrees.");
    }
}
