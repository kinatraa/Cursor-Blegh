using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneAssassin : BaseMonster
{
    [Header("Attack Settings")]
    [SerializeField] private BaseMonsterProjectile bulletPrefab;
    [SerializeField] private float bulletSpawnRadius = 0.5f;
    [SerializeField] private float delayBeforeTeleport = 2f;
    [SerializeField] private float teleportSpeed = 10f;

    [Header("Teleport Settings")]
    [SerializeField] private float fadeOutDuration = 0.5f; 
    [SerializeField] private float invisibleDuration = 2f;
    [SerializeField] private float fadeInDuration = 0.5f;
    
    protected override IEnumerator IEAttackPlayer()
    {
        _sr.color = Color.red;
        SpawnBullet();

        yield return new WaitForSeconds(delayBeforeTeleport);

        yield return StartCoroutine(IETeleportToRandomPosition());

        float totalTeleportTime = fadeOutDuration + invisibleDuration + fadeInDuration;
        float remainingTime = _remainingAnimTime - delayBeforeTeleport - totalTeleportTime;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        _sr.color = Color.white;
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

        Debug.Log("BoneAssassin spawned a bullet towards the player.");
    }

    private IEnumerator IETeleportToRandomPosition()
    {
        var waveController = GameplayManager.Instance.waveController;
        if (waveController == null){
            Debug.LogWarning("WaveController not found!");
            yield break;
        }

        float myRadius = GetMonsterRadius();

        Vector3 newPosition = FindValidTeleportPosition(waveController, myRadius);
        if (newPosition == Vector3.zero){
            Debug.LogWarning("No valid teleport position found!");
            yield break;
        }

        yield return StartCoroutine(FadeOut());

        transform.position = newPosition;

        yield return new WaitForSeconds(invisibleDuration);

        yield return StartCoroutine(FadeIn());

        Debug.Log("BoneAssassin teleported to a new position.");
    }   

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color startColor = _sr.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            _sr.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        
        _sr.color = targetColor;
    }
    
    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color startColor = _sr.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;
            _sr.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        
        _sr.color = targetColor;
    }

    private Vector3 FindValidTeleportPosition(WaveController waveController, float myRadius)
    {
        int maxAttempts = 30;
        
        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(-waveController.spawnAreaSize.x / 2, waveController.spawnAreaSize.x / 2);
            float randomY = Random.Range(-waveController.spawnAreaSize.y / 2, waveController.spawnAreaSize.y / 2);
            Vector3 candidatePos = waveController.spawnCenter + new Vector3(randomX, randomY, 0);
            
            bool overlap = false;
            var allMonsters = GameplayManager.Instance.monsterController.GetComponentsInChildren<BaseMonster>();
            
            foreach (var monster in allMonsters)
            {
                if (monster == this) continue;
                
                float otherRadius = monster.GetMonsterRadius();
                float distance = Vector3.Distance(candidatePos, monster.transform.position);
                
                if (distance < (myRadius + otherRadius))
                {
                    overlap = true;
                    break;
                }
            }
            
            if (!overlap)
            {
                return candidatePos;
            }
        }
        
        return Vector3.zero;
    }

    private float GetMonsterRadius()
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
}
