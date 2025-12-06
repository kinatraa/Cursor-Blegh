using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampiricRageBuff : BaseBuff
{
    public VampiricRageBuff(BuffData data) : base(data)
    {
    }

    public override void Dispose()
    {
        base.Dispose();
        GameplayManager.Instance.monsterController.vampiricRageBuff = null;
    }

    public void AddStack()
    {
        base.AddStack();
        GameplayManager.Instance.monsterController.vampiricRageBuff = this;
    }

    public override void Activate()
    {
        base.Activate();

        float healRate = data.activeRate;
        if (Random.value * 100 >= healRate) return;

        BaseWeapon weapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (weapon != null && weapon.currentHp < weapon.maxHp)
        {
            weapon.currentHp += 1;
            if (weapon.currentHp > weapon.maxHp)
            {
                weapon.currentHp = weapon.maxHp;
            }
            GameEventManager.InvokeUpdatePlayerHp(weapon.currentHp);
            Debug.Log("<color=green>Vampiric Rage: Healed 1 HP!</color>");
        }
    }
}
