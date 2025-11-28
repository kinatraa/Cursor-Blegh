using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInferno : BaseMonsterProjectile
{
    [Header("Inferno Settings")]
    [SerializeField] private float homingDuration = 2f; 
    [SerializeField] private float homingStrength = 1f;

    protected override IEnumerator IEProjectileMove(Vector3 targetPosition)
    {
        float timer = 0f;
        float homingTimer = 0f;
        Vector3 finalDirection = Vector3.zero;

        while (timer < existTime && isMoving)
        {
            timer += Time.deltaTime;
            homingTimer += Time.deltaTime;

            Vector3 direction;

            if (homingTimer < homingDuration)
            {
                var playerController = GameplayManager.Instance.weaponController.currentWeapon;
                if (playerController != null)
                {
                    Vector3 currentPlayerPos = playerController.transform.position;
                    Vector3 toPlayer = (currentPlayerPos - transform.position).normalized;
                    
                    Vector3 currentDir = (targetPosition - transform.position).normalized;
                    
                    direction = Vector3.Lerp(currentDir, toPlayer, homingStrength * Time.deltaTime * 5f).normalized;
                    
                    targetPosition = currentPlayerPos;
                    finalDirection = direction;
                }
                else
                {
                    direction = (targetPosition - transform.position).normalized;
                    finalDirection = direction;
                }
            }
            else
            {
                direction = finalDirection;
            }

            transform.position += direction * (speed * Time.deltaTime);

            yield return null;
        }

        Destroy();
    }
}