using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThunderStrikeBuff : BaseBuff
{
    private float _maxRange = 3f;
    
    public ThunderStrikeBuff(BuffData data) : base(data)
    {
    }

    public override void Activate()
    {
        int currentStack = stack;
        float activateChance = data.activeRate + (currentStack - 1) * 10f;

        if (Random.value * 100 >= activateChance) return;

        GameplayManager.Instance.StartCoroutine(IEActivate());
    }

    private IEnumerator IEActivate()
    {
        BaseMonster hitMonster = GameplayManager.Instance.monsterController.lastHitMonster;
        if (hitMonster == null) yield break;

        Vector3 startPosition = hitMonster.transform.position;

        List<BaseMonster> nearestMonsters = GetNearestMonsters(startPosition, hitMonster, 2, _maxRange);
        if (nearestMonsters.Count == 0) yield break;

        foreach (BaseMonster targetMonster in nearestMonsters)
        {
            if (targetMonster == null || targetMonster.isDead) continue;

            GameObject thunder = Object.Instantiate(
                data.prefab,
                targetMonster.transform.position,
                Quaternion.identity
            );

            ThunderStrikeObject thunderObject = thunder.GetComponent<ThunderStrikeObject>();
            if (thunderObject != null)
            {
                thunderObject.Strike(targetMonster.transform.position);
            }

            targetMonster.TakeDamage(data.atk);
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private List<BaseMonster> GetNearestMonsters(Vector3 position, BaseMonster excludeMonster, int count, float maxRange)
    {
        List<BaseMonster> allMonsters = GameplayManager.Instance.monsterController.MonsterList;
        
        return allMonsters
            .Where(m => m != null && !m.isDead && m != excludeMonster)
            .Where(m => Vector3.Distance(position, m.transform.position) <= maxRange)
            .OrderBy(m => Vector3.Distance(position, m.transform.position))
            .Take(count)
            .ToList();
    }
}