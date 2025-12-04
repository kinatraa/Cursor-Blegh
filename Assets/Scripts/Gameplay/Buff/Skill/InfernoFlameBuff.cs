using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfernoFlameBuff : BaseBuff
{
    public InfernoFlameBuff(BuffData data) : base(data)
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
        BaseMonster lastHitMonster = GameplayManager.Instance.monsterController.lastHitMonster;
        if (lastHitMonster == null) yield break;

        InfernoFlameObject existingFlame = lastHitMonster.GetComponentInChildren<InfernoFlameObject>();
        if (existingFlame != null) yield break;
        
        GameObject infernoFlame = Object.Instantiate(
            data.prefab, 
            lastHitMonster.transform.position, 
            Quaternion.identity
        );
        
        infernoFlame.transform.SetParent(lastHitMonster.transform);
        
        InfernoFlameObject flameObject = infernoFlame.GetComponent<InfernoFlameObject>();
        if (flameObject != null)
        {
            flameObject.Initialize(lastHitMonster);
        }
    }
}
