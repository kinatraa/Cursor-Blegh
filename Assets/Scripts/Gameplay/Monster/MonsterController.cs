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
            _monsterDict.Add(monsterPrefab.data.type, monsterPrefab);
        }
    }

    public void SpawnMonster(MonsterType type, Vector3 position)
    {
        var prefab = _monsterDict[type];
        Instantiate(prefab, position, Quaternion.identity, transform);
    }
}
