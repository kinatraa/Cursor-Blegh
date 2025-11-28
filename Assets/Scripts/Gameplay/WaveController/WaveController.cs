using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveController : MonoBehaviour
{
    [SerializeField] private TextAsset waveConfigFile;
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    public Vector3 spawnCenter = Vector3.zero;

    [Header("Stats")]
    public int currentWave = 1; 

    private WaveCollection _waveData;
    private int _maxConfiguredWave = 0; 
    
    private MonsterController _monsterController;

    void Start()
    {
        _monsterController = GameplayManager.Instance.monsterController;
        LoadWaveConfig();
        SimulateSpawnLogic();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentWave++;
            SimulateSpawnLogic();
        }
    }

    void LoadWaveConfig()
    {
        if (waveConfigFile != null)
        {
            _waveData = JsonUtility.FromJson<WaveCollection>(waveConfigFile.text);
            
            if (_waveData.waves != null && _waveData.waves.Count > 0)
            {
                _maxConfiguredWave = _waveData.waves.Max(w => w.wave_max);
            }
        }
    }

    void SimulateSpawnLogic()
    {
        if (_waveData == null || _maxConfiguredWave == 0) return;
        
        int effectiveWave = currentWave; 

        if (currentWave > _maxConfiguredWave)
        {
            effectiveWave = ((currentWave - 1) % _maxConfiguredWave) + 1;
        }
        
        WaveConfig config = _waveData.waves.Find(w => effectiveWave >= w.wave_min && effectiveWave <= w.wave_max);
        List<SpawnData> currentBatchSpawns = new List<SpawnData>();
        if (config == null)
        {
            Debug.LogWarning($"Not found wave config: {effectiveWave}");
            return;
        }

        if (config.is_boss_wave)
        {
            foreach (var enemy in config.fixed_enemies)
            {
                MonsterType mType = ParseMonsterType(enemy.id);
                MonsterSize mSize = ParseMonsterSize(enemy.size);
                float radius = _monsterController.GetMonsterRadius(mType, mSize);

                if (!_monsterController.HasMonster(mType, mSize))
                {
                    Debug.LogWarning($"<color=red>Prefab [{enemy.id} - {enemy.size}] not found in MonsterController!</color>");
                    continue;
                }
                
                for (int i = 0; i < enemy.count; i++)
                {
                    Vector3 pos = GetValidSpawnPosition(radius, currentBatchSpawns);
                    if (pos != Vector3.zero)
                    {
                        _monsterController.SpawnMonster(mType, pos, mSize);
                        
                        currentBatchSpawns.Add(new SpawnData{position = pos, radius = radius});

                        Debug.Log($"Generate [{enemy.id} - {enemy.size}] at {FormatVector(pos)}");
                    }
                    else
                    {
                        Debug.LogWarning($"Không tìm được chỗ trống cho {enemy.id}!");
                    }
                }
            }
        }
        else
        {
            int totalToSpawn = Random.Range(config.count_min, config.count_max + 1); 
            for (int i = 0; i < totalToSpawn; i++)
            {
                EnemySpawnRate chosen = PickEnemyByRatio(config.enemies);
                if (chosen != null)
                {
                    MonsterType mType = ParseMonsterType(chosen.id);
                    MonsterSize mSize = ParseMonsterSize(chosen.size);
                    float radius = _monsterController.GetMonsterRadius(mType, mSize);

                    if (!_monsterController.HasMonster(mType, mSize))
                    {
                        Debug.LogWarning($"<color=red>Prefab [{chosen.id} - {chosen.size}] not found in MonsterController!</color>");
                        continue;
                    }
                    
                    Vector3 pos = GetValidSpawnPosition(radius, currentBatchSpawns);
                    
                    if (pos != Vector3.zero)
                    {
                        _monsterController.SpawnMonster(mType, pos, mSize);
                        currentBatchSpawns.Add(new SpawnData { position = pos, radius = radius });
                        Debug.Log($"[{i + 1}/{totalToSpawn}] Sinh [{chosen.id} - {chosen.size}] tại {FormatVector(pos)}");
                    }
                }
            }
        }
        Debug.Log("========================================\n");
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

    EnemySpawnRate PickEnemyByRatio(List<EnemySpawnRate> enemies)
    {
        int totalRatio = enemies.Sum(e => e.ratio);
        int randomVal = Random.Range(0, totalRatio);
        int currentSum = 0;
        foreach (var enemy in enemies)
        {
            currentSum += enemy.ratio;
            if (randomVal < currentSum) return enemy;
        }
        return enemies.LastOrDefault();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnCenter, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1));
    }
}

public struct SpawnData
{
    public Vector3 position;
    public float radius;
}
[System.Serializable] public class WaveCollection { public List<WaveConfig> waves; }
[System.Serializable] public class WaveConfig { 
    public int wave_min; public int wave_max; 
    public int count_min; public int count_max; 
    public bool is_boss_wave; 
    public List<EnemySpawnRate> enemies; 
    public List<EnemyFixedSpawn> fixed_enemies; 
}
[System.Serializable] public class EnemySpawnRate { public string id; public string size; public int ratio; }
[System.Serializable] public class EnemyFixedSpawn { public string id; public string size; public int count; }