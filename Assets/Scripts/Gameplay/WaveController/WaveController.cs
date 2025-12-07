using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class WaveController : MonoBehaviour
{
    [SerializeField] private TextAsset _waveConfigFile;
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    public Vector3 spawnCenter = Vector3.zero;

    [Header("Stats")]
    public int currentWave = 1; 

    private WaveCollection _waveData;
    private int _maxConfiguredWave = 0; 
    private const int BOSS_WAVE_INTERVAL = 30;
    
    private MonsterController _monsterController;
    private void Awake()
    {
        _monsterController = GameplayManager.Instance.monsterController;
        LoadWaveConfig();
    }

    public void SetCurrentWave(int wave)
    {
        currentWave = wave;
        GameEventManager.InvokeUpdateWave(currentWave);
    }

    private void LoadWaveConfig()
    {
        if (_waveConfigFile == null) return;
        _waveData = JsonUtility.FromJson<WaveCollection>(_waveConfigFile.text);
            
        if (_waveData.waves is { Count: > 0 })
        {
            _maxConfiguredWave = _waveData.waves.Max(w => w.wave_max);
        }
    }

    private bool IsBossWave(int wave)
    {
        if (wave == 40) return true;
        if ((wave > 40) && (wave - 40) % BOSS_WAVE_INTERVAL == 0) return true;
        return false;
    }

    private int GetEffectiveWave(int wave)
    {
        if (wave <= _maxConfiguredWave) return wave;
        
        int offset = wave - _maxConfiguredWave;
        int cycleLength = 9;
        int cycleWave = ((offset - 1) % cycleLength) + 31;
        return cycleWave;
    }

    public void SpawnMonsters()
    {
        if (_waveData == null || _maxConfiguredWave == 0) return;
        
        WaveConfig config = null;
        
        if (IsBossWave(currentWave))
        {
            AudioManager.Instance.PlayMusic("bgm_bossfight" + Random.Range(1, 3), volume: 0.5f);
            config = _waveData.waves.Find(w => w.wave_min == 40 && w.wave_max == 40);
            Debug.Log($"<color=yellow>=== BOSS WAVE {currentWave} ===</color>");
        }
        else
        {
            AudioManager.Instance.PlayMusic("bgm_gameplay", volume: 0.5f);
            int effectiveWave = GetEffectiveWave(currentWave);
            config = _waveData.waves.Find(w => effectiveWave >= w.wave_min && effectiveWave <= w.wave_max);
            Debug.Log($"=== Start Wave {currentWave} (Config Wave: {effectiveWave}) ===");
        }

        if (config == null)
        {
            Debug.LogWarning("Not found wave config");
            return;
        }

        GameplayManager.Instance.waveRewardSystem.StartWave();
        
        List<MonsterSpawnInfo> spawnQueue = new List<MonsterSpawnInfo>();

        foreach (var enemyConfig in config.enemies)
        {
            int countToSpawn = Random.Range(enemyConfig.min_count, enemyConfig.max_count + 1);

            for (int i = 0; i < countToSpawn; i++)
            {
                spawnQueue.Add(new MonsterSpawnInfo 
                { 
                    id = enemyConfig.id, 
                    size = enemyConfig.size 
                });
            }
        }
        
        ShuffleList(spawnQueue);
        
        List<SpawnData> currentBatchSpawns = new List<SpawnData>();
        
        for (int i = 0; i < spawnQueue.Count; i++)
        {
            var info = spawnQueue[i];
            MonsterType mType = ParseMonsterType(info.id);
            MonsterSize mSize = ParseMonsterSize(info.size);
            
            if (!_monsterController.HasMonster(mType, mSize))
            {
                Debug.LogWarning($"<color=red>Prefab [{info.id} - {info.size}] not found in MonsterController!</color>");
                continue;
            }

            float radius = _monsterController.GetMonsterRadius(mType, mSize);
            Vector3 pos = GetValidSpawnPosition(radius, currentBatchSpawns);

            if (pos != Vector3.zero)
            {
                _monsterController.SpawnMonster(mType, pos, mSize, currentWave);
                currentBatchSpawns.Add(new SpawnData { position = pos, radius = radius });
                Debug.Log($"[{i + 1}/{spawnQueue.Count}] Sinh [{info.id} - {info.size}] tại {FormatVector(pos)}");
            }
            else
            {
                Debug.LogWarning($"Can't find position for {info.id}!");
            }
        }
        
        string hitKey = "item_choose";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }
        
        Debug.Log("========================================\n");
    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    
    Vector3 GetValidSpawnPosition(float myRadius, List<SpawnData> existingSpawns)
    {
        int maxAttempts = 30;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 candidatePos = GetRandomPosition();
            bool overlap = false;
            foreach (var other in existingSpawns)
            {
                float distance = Vector3.Distance(candidatePos, other.position);
                if (distance < (myRadius + other.radius)) 
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
    
    Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return spawnCenter + new Vector3(randomX, randomY, 0);
    }

    string FormatVector(Vector3 v) => $"({v.x:F1}, {v.y:F1}, {v.z:F1})";
    
    MonsterType ParseMonsterType(string monsterType)
    {
        if (System.Enum.TryParse<MonsterType>(monsterType, true, out MonsterType result)) return result;
        return MonsterType.NONE;
    }

    MonsterSize ParseMonsterSize(string monsterSize)
    {
        if (System.Enum.TryParse<MonsterSize>(monsterSize, true, out MonsterSize result)) return result;
        return MonsterSize.SMALL;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnCenter, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1));
    }

    public void Reset()
    {
        
    }
}

public struct SpawnData
{
    public Vector3 position;
    public float radius;
}

public class MonsterSpawnInfo 
{
    public string id;
    public string size;
}

[System.Serializable]
public class WaveCollection
{
    public List<WaveConfig> waves;
}

[System.Serializable] public class WaveConfig { 
    public int wave_min; public int wave_max; 
    public int count_min; public int count_max; 
    public bool is_boss_wave; 
    public List<EnemyConfig> enemies; 
}
[System.Serializable]
public class EnemyConfig
{
    public string id;
    public string size;
    public int min_count;
    public int max_count;
}