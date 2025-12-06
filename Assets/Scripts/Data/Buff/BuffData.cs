using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffData", menuName = "Game/Buff Data")]
public class BuffData : ScriptableObject
{
    [Header("Visuals")] 
    public string buffName;
    public Sprite buffIcon;
    [TextArea] public string description;
    
    [Header("Identity")]
    public BuffType type;
    public BuffEffectType effectType;
    public CollectType collectType;
    public GameObject prefab;
    
    [Header("Stats")]
    public int maxStack;
    public float activeRate;
    public int atk;
}

public enum BuffType
{
    NONE = 0,
    SHURIKEN = 1,
    SENTINEL_SHIELD = 2,
    TRUE_SHOT = 3,
    POISON_THORN = 4,
    THUNDER_STRIKE = 5,
    FROST_PRISON = 6,
    INFERNO_FLAME = 7,
    MOLTEN_STONE = 8,
    LIFE_RESET = 9,
    VAMPIRIC_RAGE = 10,
    IMMORTAL = 11,
    HEALING_HERB = 12,
    REBORN = 13,
    THICK_CLOTH = 14,
    PRISM_LENS = 15,
    ARCANE_TOME = 16,
    SHARPEN_STONE = 17,
    MEDIC_BANDAGE = 18,
    FAIRY_ELIXIR = 19,
    MYSTIC_POTION = 20,
}

public enum BuffEffectType
{
    NONE = 0,
    SKILL = 1,
    ITEM = 2,
}

public enum CollectType
{
    CAN_COLLECT = 0,
    CAN_NOT_COLLECT = 1,
}
