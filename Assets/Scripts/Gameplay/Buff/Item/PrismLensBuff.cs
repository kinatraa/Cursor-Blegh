using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismLensBuff : BaseBuff
{
    public PrismLensBuff(BuffData data) : base(data)
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

        weapon.critDmgToAdd += 3f;
    }
}
