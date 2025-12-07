using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneTomeBuff : BaseBuff
{
    public ArcaneTomeBuff(BuffData data) : base(data)
    {
    }
    public override void AddStack()
    {
        base.AddStack();
        Activate();
    }

    public override void Activate()
    {
        BaseWeapon weapon = GameplayManager.Instance.weaponController.currentWeapon;
        if (weapon == null) return;

        weapon.critChanceToAdd += 3f;
    }
}
