using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthSkill : BaseWeaponSkill
{
    public StealthSkill(WeaponSkillData data) : base(data)
    {
    }

    public override void Activate(BaseWeapon weapon)
    {
        base.Activate(weapon);
    }
}
