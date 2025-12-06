using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public List<BaseMonster> monsterPrefabs = new List<BaseMonster>();

    private Dictionary<(MonsterType type, MonsterSize size), BaseMonster> _monsterDict =
        new Dictionary<(MonsterType type, MonsterSize size), BaseMonster>();

    private Dictionary<MonsterSize, float> _sizeMultipliers = new Dictionary<MonsterSize, float>
    {
        { MonsterSize.SMALL, 0.7f },
        { MonsterSize.MEDIUM, 1.0f },
        { MonsterSize.LARGE, 1.5f }
    };

    private List<BaseMonster> _monsterList = new List<BaseMonster>();
    public List<BaseMonster> MonsterList => _monsterList;

    public BaseMonster lastHitMonster = null;
    public HealingHerbBuff healingHerbBuff = null;
    public MysticPotionBuff mysticPotionBuff = null;
    public VampiricRageBuff vampiricRageBuff = null;

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
        var newMonster = Instantiate(prefab, position, Quaternion.identity, transform);
        _monsterList.Add(newMonster);

        Debug.Log($"Spawned {newMonster.data.monsterName} [{type} - {size}] at {position}");
    }

    public void SpawnMonster(MonsterType type, Vector3 position, MonsterSize size, int currentWave)
    {
        var key = (type, size);

        if (!_monsterDict.ContainsKey(key))
        {
            Debug.LogError($"Monster [{type} - {size}] not found in prefab list!");
            return;
        }

        var prefab = _monsterDict[key];
        var newMonster = Instantiate(prefab, position, Quaternion.identity, transform);
        if (currentWave > 40)
        {
            float factor = 1;
            if (!(currentWave <= 70))
            {
                factor = ((currentWave - 40) / 30) + factor;
            }

            UpgradeMonster(newMonster, factor);
        }

        _monsterList.Add(newMonster);

        Debug.Log($"Spawned {newMonster.data.monsterName} [{type} - {size}] at {position}");
    }

    private void UpgradeMonster(BaseMonster newMonster, float factor)
    {
        newMonster.currentHp = (int)(1.2 * factor);
        if (!(newMonster.projectileSpeed + 5 * factor >= 50)) newMonster.projectileSpeed += 5 * factor;
        if (!(newMonster.reduceAnimTime + 0.1 * factor >= 0.5)) newMonster.reduceAnimTime += 0.1f * factor;
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

    public void RemoveMonster(BaseMonster monster)
    {
        _monsterList.Remove(monster);

        if (AllMonsterAreCleared())
        {
            GameplayManager.Instance.stateController.Next();
        }
    }


    public bool AllMonsterAreCleared()
    {
        return _monsterList.Count == 0;
    }

    public void Reset()
    {
        if (_monsterList != null)
        {
            foreach (var monster in _monsterList)
            {
                Destroy(monster.gameObject);
            }
        }
        _monsterList = new List<BaseMonster>();

        lastHitMonster = null;
        healingHerbBuff = null;
        mysticPotionBuff = null;
        vampiricRageBuff = null;
    }
}