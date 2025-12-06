using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithSorcerer : BaseMonster
{
    [Header("Wraith Sorcerer")]
    [SerializeField] private List<BaseMonster> _monstersList = new List<BaseMonster>();
    [SerializeField] private Color _summonColor = new Color(0.8f, 0.3f, 0.8f);

    protected override IEnumerator IEAttackPlayer()
    {
        PlayAnimation(ANIM_ATTACK);

        _sr.color = _summonColor;
        
        string hitKey = "monster_summon";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }

        SummonMonster();
        yield return new WaitForSeconds(_remainingAnimTime);
        
        _sr.color = Color.white;
    }

    private void SummonMonster()
    {
        if (_monstersList == null || _monstersList.Count == 0)
        {
            Debug.Log("Monster list is empty");
            return;
        }

        var waveController = GameplayManager.Instance.waveController;
        if (waveController == null)
        {
            Debug.LogWarning("Wave controller is null");
            return;
        }
        
        BaseMonster randomPrefab = _monstersList[Random.Range(0, _monstersList.Count)];

        float spawnRadius = GetMonsterRadius(randomPrefab);

        Vector3 spawnPosition = FindValidSpawnPosition(waveController, spawnRadius);

        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning("Spawn position is invalid");
            return;
        }
        
        var summonMonster = Instantiate(randomPrefab, spawnPosition, Quaternion.identity);
        int currentWave = GameplayManager.Instance.waveController.currentWave;
        UpgradeMonster(currentWave, summonMonster);
        summonMonster.transform.SetParent(GameplayManager.Instance.monsterController.transform);
        
        Debug.Log($"Wraith Sorcerer summoned {summonMonster.data.monsterName}");
    }

    private void UpgradeMonster(int currentWave, BaseMonster summonMonster)
    {
        if (currentWave > 40)
        {
            float factor = 1;
            if (!(currentWave <= 70))
            {
                factor = ((currentWave - 40) / 30) + factor;
            }
            summonMonster.currentHp = (int)(1.2 * factor);
            if (!(summonMonster.projectileSpeed + 5*factor >= 50)) summonMonster.projectileSpeed += 5 * factor;
            if (!(summonMonster.reduceAnimTime + 0.1*factor >= 0.5))  summonMonster.reduceAnimTime += 0.1f * factor;
        }
    }

    private Vector3 FindValidSpawnPosition(WaveController waveController, float spawnRadius)
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
                float otherRadius = monster.GetMonsterRadius();
                float distance = Vector3.Distance(candidatePos, monster.transform.position);
                
                if (distance < (spawnRadius + otherRadius))
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

    private float GetMonsterRadius(BaseMonster monsterPrefab)
    {
        var spriteRenderer = monsterPrefab.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            return Mathf.Max(
                spriteRenderer.sprite.bounds.size.x * monsterPrefab.transform.localScale.x,
                spriteRenderer.sprite.bounds.size.y * monsterPrefab.transform.localScale.y
            ) * 0.5f;
        }
        return 0.5f;
    }
}
