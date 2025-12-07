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
        float activateChance = data.activeRate + (currentStack - 1) * 3f;
        
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
        
        string hitKey = "buff_frost";
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }
        
        frostPrison.transform.SetParent(lastHitMonster.transform);

        lastHitMonster.Freeze(frostTime);
    }
}
