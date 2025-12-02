using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostPrisonBuff : BaseBuff
{
    public FrostPrisonBuff(BuffData data) : base(data)
    {
    }

    public override void Activate()
    {
        int currentStack = stack;
        float activateChance = data.activeRate + (currentStack - 1) * 5f;
        
        if (Random.value * 100 >= activateChance) return;
        
        GameplayManager.Instance.StartCoroutine(IEActivate());
    }

    private IEnumerator IEActivate()
    {
        BaseMonster lastHitMonster = GameplayManager.Instance.monsterController.lastHitMonster;
        if (lastHitMonster == null) yield break;

        if (lastHitMonster.isFrozen == true) yield break;
        
        GameObject frostPrison = Object.Instantiate(
            data.prefab, 
            lastHitMonster.transform.position, 
            Quaternion.identity
        );
        
        float frostTime = frostPrison.GetComponent<FrostPrisonObject>()._frostDuration;
        
        frostPrison.transform.SetParent(lastHitMonster.transform);

        lastHitMonster.Freeze(frostTime);
    }
}
