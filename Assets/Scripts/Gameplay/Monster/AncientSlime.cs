using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AncientSlime : BaseMonster
{
    protected override void Die()
    {
        if (data.size == MonsterSize.LARGE)
        {
            SpawnChildren(MonsterSize.MEDIUM, 4, 1f);
        }
        else if (data.size == MonsterSize.MEDIUM)
        {
            SpawnChildren(MonsterSize.SMALL , 4, 0.5f);
        }

        base.Die();
    }

    void SpawnChildren(MonsterSize size, int count, float offset)
    {
        Vector2 center = transform.position;

        Vector2[] positions =
        {
            center + new Vector2(-offset,  offset),
            center + new Vector2( offset,  offset),
            center + new Vector2(-offset, -offset),
            center + new Vector2( offset, -offset)
        };

        var childPrefab = GameplayManager.Instance.monsterController.monsterPrefabs
            .FirstOrDefault(m => m.data.type == MonsterType.ANCIENT_SLIME 
                                 && m.data.size == size);
        foreach (var pos in positions)
        {
            Instantiate(childPrefab, pos, Quaternion.identity);
        }
    }
}
