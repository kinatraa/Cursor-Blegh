using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AncientSlime : BaseMonster
{
    protected override IEnumerator IEDieSequence()
    {
        PlayAnimation(ANIM_DIE); 
        
        yield return null; 
        
        float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        
        if (data.size == MonsterSize.LARGE)
        {
            SpawnChildren(MonsterSize.MEDIUM, 4, 1f);
        }
        else if (data.size == MonsterSize.MEDIUM)
        {
            SpawnChildren(MonsterSize.SMALL , 4, 0.5f);
        }
        
        Destroy(gameObject);
    }

    private void SpawnChildren(MonsterSize size, int count, float offset)
    {
        Vector2 center = transform.position;

        Vector2[] positions =
        {
            center + new Vector2(-offset,  offset),
            center + new Vector2( offset,  offset),
            center + new Vector2(-offset, -offset),
            center + new Vector2( offset, -offset)
        };

        foreach (var pos in positions)
        {
            GameplayManager.Instance.monsterController.SpawnMonster(MonsterType.SLIME_PRIMORDIAL, pos, size);
        }
    }
}
