using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueShotBuff : BaseBuff
{
    private int _damageMultiplier = 3;
    public TrueShotBuff(BuffData data) : base(data)
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

        int weaponDamage = GameplayManager.Instance.weaponController.currentWeapon.data.atk;
        
        lastHitMonster.TakeDamage(weaponDamage * (_damageMultiplier - 1));
    }
}
