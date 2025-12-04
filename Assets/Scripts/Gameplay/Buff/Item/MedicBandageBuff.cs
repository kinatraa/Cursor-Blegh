using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicBandageBuff : BaseBuff
{
    public MedicBandageBuff(BuffData data) : base(data)
    {
    }

    public override void AddStack()
    {
        BaseWeapon weapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (weapon == null)
        {
            return;
        }

        if (weapon.currentHp == weapon.maxHp)
        {
            if (stack < data.maxStack)
            {
                stack++;
                weapon.maxHp++;
                weapon.currentHp = weapon.maxHp;
            }
        }
        else
        {
            weapon.currentHp++;
        }
        GameEventManager.InvokeUpdatePlayerMaxHp(weapon.maxHp);
        GameEventManager.InvokeUpdatePlayerHp(weapon.currentHp);
    }
}
