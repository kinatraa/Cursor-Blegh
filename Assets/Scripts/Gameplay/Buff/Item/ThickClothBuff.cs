using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickClothBuff : BaseBuff
{
    public ThickClothBuff(BuffData data) : base(data)
    {
    }

    public override void AddStack()
    {
        if (stack < 4){
            base.AddStack();
            Activate();
        }
    }

    public override void Activate()
    {
        BaseWeapon weapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (weapon == null)
        {
            return;
        }

        weapon.maxHp += 1;
        if (weapon.currentHp < weapon.maxHp) weapon.currentHp += 1;
        GameEventManager.InvokeUpdatePlayerMaxHp(weapon.maxHp);
        GameEventManager.InvokeUpdatePlayerHp(weapon.currentHp);
    }
}
