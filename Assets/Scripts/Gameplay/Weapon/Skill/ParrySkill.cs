using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrySkill : BaseWeaponSkill
{
    public ParrySkill(WeaponSkillData data) : base(data)
    {
    }

    public override void Activate(BaseWeapon weapon)
    {
        base.Activate(weapon);
    }
}
