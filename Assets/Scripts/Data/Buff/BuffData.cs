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
    
    [Header("Stats")]
    public int maxStack;
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
