using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public List<BaseMonster> monsterPrefabs = new List<BaseMonster>();

    private Dictionary<MonsterType, BaseMonster> _monsterDict = new Dictionary<MonsterType, BaseMonster>();
    
    private void Awake()
    {
        foreach (var monsterPrefab in monsterPrefabs)
        {
            _monsterDict.TryAdd(monsterPrefab.data.type, monsterPrefab);
        }
    }

    public void SpawnMonster(MonsterType type, Vector3 position)
    {
        if (!_monsterDict.ContainsKey(type))
        {
            Debug.LogError($"Monster {type} not found");
            return;
        }
        
        var prefab = _monsterDict[type];
        Instantiate(prefab, position, Quaternion.identity, transform);
    }
    
    public void SpawnMonster(string typeStr, Vector3 position)
    {
        MonsterType type = MonsterType.NONE;
        foreach (MonsterType t in System.Enum.GetValues(typeof(MonsterType)))
        {
            if (t.ToString() == typeStr)
            {
                type = t;
                break;
            }
        }

        if (!_monsterDict.ContainsKey(type))
        {
            Debug.LogError($"Monster {typeStr} not found");
            return;
        }
        
        var prefab = _monsterDict[type];
        Instantiate(prefab, position, Quaternion.identity, transform);
    }
}
