using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarEye : BaseMonster
{
    [Header("Laser Attack Settings")]
    [SerializeField] private LaserBeam _laserPrefab;
    [SerializeField] private int _laserCount = 4;
    [SerializeField] private float _laserLength = 30f;
    [SerializeField] private bool _randomAngles = false; 
    [SerializeField] private float _startAngleOffset = 0f;
    
    protected override IEnumerator IEAttackPlayer()
    {
        PlayAnimation(ANIM_ATTACK);
        _sr.color = Color.yellow;
        
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

        if (_randomAngles)
        {
            for (int i = 0; i < _laserCount; i++)
            {
                float randomAngle = Random.Range(0f, 360f);
                SpawnLaser(randomAngle);
            }
        }
        else
        {
            float angleStep = 360f / _laserCount;
            
            for (int i = 0; i < _laserCount; i++)
            {
                float angle = (i * angleStep) + _startAngleOffset;
                SpawnLaser(angle);
            }
        }
        
        Debug.Log($"<color=cyan>SolarEye fired {_laserCount} lasers!</color>");
    }

    private void SpawnLaser(float angleDegrees)
    {
        var laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        
        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians), 0f);
        
        laser.Initialize(transform.position, direction, _laserLength);
        
        Debug.Log($"<color=yellow>Laser spawned at angle {angleDegrees}°</color>");
    }
}