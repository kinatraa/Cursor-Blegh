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
    ANCIENT_SLIME = 1,
    INFERNO_BEAST = 2,
    BONE_MARKSMAN = 3,
    BONE_ASSASSIN = 4,
    SOLAR_EYE = 5,
    THUNDER_BEAST = 6,
    TOXIC_SLIME = 7,
    FLAME_SLIME = 8,
    WRAITH_SORCERER = 9,
    DRAKE_BEAST = 10,
    DEMON_KING = 11,
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