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

    private WaveCollection waveData;
    private int maxConfiguredWave = 0; 

    void Start()
    {
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
            waveData = JsonUtility.FromJson<WaveCollection>(waveConfigFile.text);
            
            if (waveData.waves != null && waveData.waves.Count > 0)
            {
                maxConfiguredWave = waveData.waves.Max(w => w.wave_max);
            }
        }
    }

    void SimulateSpawnLogic()
    {
        if (waveData == null || maxConfiguredWave == 0) return;
        
        int effectiveWave = currentWave; 
        int cycleIndex = 0;              

        if (currentWave > maxConfiguredWave)
        {
            effectiveWave = ((currentWave - 1) % maxConfiguredWave) + 1;
            cycleIndex = (currentWave - 1) / maxConfiguredWave;
        }
        
        WaveConfig config = waveData.waves.Find(w => effectiveWave >= w.wave_min && effectiveWave <= w.wave_max);

        if (config == null)
        {
            Debug.LogWarning($"Not found wave config: {effectiveWave}");
            return;
        }
        
        string prefix = cycleIndex > 0 ? $"<color=magenta>[CYCLE {cycleIndex}]</color> " : "";

        if (config.is_boss_wave)
        {
            foreach (var enemy in config.fixed_enemies)
            {
                for (int i = 0; i < enemy.count; i++)
                {
                    Vector3 pos = GetRandomPosition();
                    Debug.Log($" Generate [{enemy.id} - {enemy.size}] at {FormatVector(pos)}");
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
                    Vector3 pos = GetRandomPosition();
                    Debug.Log($"[{i + 1}/{totalToSpawn}] Sinh [{chosen.id} - {chosen.size}] tại {FormatVector(pos)}");
                }
            }
        }
        Debug.Log("========================================\n");
    }
    
    Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return spawnCenter + new Vector3(randomX, 0, randomZ);
    }

    string FormatVector(Vector3 v) => $"({v.x:F1}, {v.y:F1}, {v.z:F1})";

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
        Gizmos.DrawWireCube(spawnCenter, new Vector3(spawnAreaSize.x, 1, spawnAreaSize.y));
    }
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