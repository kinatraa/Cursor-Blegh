using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingHerbBuff : BaseBuff
{
    public HealingHerbBuff(BuffData data) : base(data)
    {
        
    }

    public override void Dispose()
    {
        base.Dispose();

        GameplayManager.Instance.monsterController.healingHerbBuff = null;
    }

    public override void AddStack()
    {
        base.AddStack();

        GameplayManager.Instance.monsterController.healingHerbBuff = this;
    }

    public override void Activate()
    {
        base.Activate();

        float activeRate = data.activeRate + (stack - 1) * 5f;
        if (Random.value * 100 >= activeRate) return;

        var spawnPos = GameplayManager.Instance.monsterController.lastHitMonster.transform.position;
        Object.Instantiate(data.prefab, spawnPos, Quaternion.identity);
    }
}
