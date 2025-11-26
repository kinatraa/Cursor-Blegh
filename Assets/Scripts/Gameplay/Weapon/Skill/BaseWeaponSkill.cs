using UnityEngine;

public abstract class BaseWeaponSkill
{
    protected WeaponSkillData data;

    public BaseWeaponSkill(WeaponSkillData data)
    {
        this.data = data;
    }

    public virtual void Activate(BaseWeapon owner)
    {
        Debug.Log($"Data : {data.type}, {data.cooldown}, {data.range}, {data.duration}");
    }
}