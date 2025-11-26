using UnityEngine;
using System.Collections.Generic;

public class WeaponSkillSystem
{
    private Dictionary<WeaponSkillType, WeaponSkillData> _skillDataDict;
    private Dictionary<WeaponSkillType, BaseWeaponSkill> _skillLogicDict;

    public WeaponSkillSystem(List<WeaponSkillData> allSkills)
    {
        _skillDataDict = new Dictionary<WeaponSkillType, WeaponSkillData>();

        foreach (var s in allSkills)
        {
            _skillDataDict[s.type] = s;
        }

        _skillLogicDict = new Dictionary<WeaponSkillType, BaseWeaponSkill>();
    }

    public BaseWeaponSkill GetSkill(WeaponSkillType type)
    {
        if (!_skillLogicDict.ContainsKey(type))
        {
            var data = _skillDataDict[type];

            switch (type)
            {
                case WeaponSkillType.SPIN_SPLASH:
                    _skillLogicDict[type] = new SpinSplashSkill(data);
                    break;
                case WeaponSkillType.STEALTH:
                    _skillLogicDict[type] = new StealthSkill(data);
                    break;
                case WeaponSkillType.EARTHQUAKE:
                    _skillLogicDict[type] = new EarthquakeSkill(data);
                    break;
                case WeaponSkillType.PARRY:
                    _skillLogicDict[type] = new ParrySkill(data);
                    break;
            }
        }

        return _skillLogicDict[type];
    }
}