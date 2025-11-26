using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;

    [Header("Stats")] 
    public int hp;
    public int atk;
    public float crit;
    public float critDmg;

    [Header("Visuals")]
    public Sprite icon;
    
    [Header("Skills")]
    public WeaponSkillType skill;
}

public enum WeaponType
{
    NONE = 0,
    WOODEN_SWORD = 1,
    DAGGER = 2,
    BATTLE_AXE = 3,
    PREMIUM_SWORD = 4,
}

