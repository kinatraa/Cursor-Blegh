using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public List<BaseMonster> monsterPrefabs = new List<BaseMonster>();

    // Dictionary để cache theo type và size
    private Dictionary<(MonsterType type, MonsterSize size), BaseMonster> _monsterDict = 
        new Dictionary<(MonsterType type, MonsterSize size), BaseMonster>();
    
    private Dictionary<MonsterSize, float> _sizeMultipliers = new Dictionary<MonsterSize, float>
    {
        { MonsterSize.SMALL, 0.7f },
        { MonsterSize.MEDIUM, 1.0f },
        { MonsterSize.LARGE, 1.5f }
    };

    private void Awake()
    {
        foreach (var monsterPrefab in monsterPrefabs)
        {
            var key = (monsterPrefab.data.type, monsterPrefab.data.size);
            if (!_monsterDict.ContainsKey(key))
            {
                _monsterDict.Add(key, monsterPrefab);
            }
        }
    }

    public void SpawnMonster(MonsterType type, Vector3 position, MonsterSize size)
    {
        var key = (type, size);
        
        if (!_monsterDict.ContainsKey(key))
        {
            Debug.LogError($"Monster [{type} - {size}] not found in prefab list!");
            return;
        }

        var prefab = _monsterDict[key];
        var instance = Instantiate(prefab, position, Quaternion.identity, transform);
        
        Debug.Log($"Spawned {instance.data.monsterName} [{type} - {size}] at {position}");
    }
    
    public float GetMonsterRadius(MonsterType type, MonsterSize size)
    {
        var key = (type, size);
        
        if (!_monsterDict.ContainsKey(key))
        {
            Debug.LogWarning($"Monster [{type} - {size}] not found in dictionary");
            return 0.5f;
        }
        
        var prefab = _monsterDict[key];
        var spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        
        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogWarning($"SpriteRenderer not found for [{type} - {size}]");
            return 0.5f;
        }
        
        // Lấy kích thước sprite từ prefab (đã có sẵn scale và size)
        float radius = Mathf.Max(
            spriteRenderer.sprite.bounds.size.x * spriteRenderer.transform.localScale.x,
            spriteRenderer.sprite.bounds.size.y * spriteRenderer.transform.localScale.y
        ) * 0.5f;
        
        return radius;
    }
    
    public bool HasMonster(MonsterType type, MonsterSize size)
    {
        return _monsterDict.ContainsKey((type, size));
    }
}