using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonThornBuff : BaseBuff
{
    public PoisonThornBuff(BuffData data) : base(data)
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
        BaseMonster killedMonster = GameplayManager.Instance.monsterController.lastHitMonster;
        if (killedMonster == null || !killedMonster.isDead) yield break;

        Vector3 spawnPosition = killedMonster.transform.position;

        Vector2[] directions = new Vector2[]{
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (Vector2 dir in directions)
        {

            GameObject thorn = Object.Instantiate(
                data.prefab,
                spawnPosition,
                Quaternion.identity
            );

            PoisonThornObject thornObject = thorn.GetComponent<PoisonThornObject>();
            if (thornObject != null)
            {
                thornObject.Initialize(dir, data.atk);
            }
        }

        yield return null;  
    }
}
