using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSkillData", menuName = "Game/Skill Data")]
public class WeaponSkillData : ScriptableObject
{
    public WeaponSkillType type;
    public float cooldown;
    public int range;
    public float duration;
}

public enum WeaponSkillType
{
    NONE = 0,
    SPIN_SPLASH = 1,
    STEALTH = 2,
    EARTHQUAKE = 3,
    PARRY = 4,
}