using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Game/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("Visuals")]
    public string monsterName;
    [TextArea] public string description;

    [Header("Identity")]
    public MonsterType type;
    public MonsterSize size;
    public MonsterRank rank;

    [Header("Stats")]
    public int hp;
    public int score;

    [Header("Combat")]
    public AttackType attackType;
}

public enum MonsterType
{
    NONE = 0,
    SLIME_PRIMORDIAL = 1,
    INFERNO_BEAST = 2,
    ELF_MARKSMAN = 3,
    ELF_ASSASSIN = 4,
    SKELETON_KING = 5,
    EMBER_MUSHROOM = 6,
    TOXIC_MUSHROOM = 7,
    SOLAR_EYE = 8,
    THUNDER_BEAST = 9,
    TOXIC_SLIME = 10,
    FLAME_SLIME = 11,
    WRAITH_SORCERER = 12,
    DRAKE_BEAST = 13,
    DEMON_KING = 14,
}

public enum AttackType
{
    MELEE = 0,
    RANGED = 1,
    HYBRID = 2
}

public enum MonsterRank
{
    NORMAL = 0,
    ELITE = 1,
    BOSS = 2
}

public enum MonsterSize
{
    SMALL = 0,
    MEDIUM = 1,
    LARGE = 2,
}