using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CombatResolver
{
    public static void CollisionResolve(BaseWeapon weapon, BaseMonster monster)
    {
        if (!weapon || !monster) return;

        if (weapon.currentState == WeaponState.BLINK) return;
        
        List<string> hitSounds = new List<string> { "attack_1", "attack_2", "weapon_swish", "weapon_swoosh" };
        int randomChance = UnityEngine.Random.Range(0, hitSounds.Count);
        string hitKey = hitSounds[randomChance];
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ShotSfx(hitKey);
        }
        
        int damage = weapon.data.atk + weapon.damageToAdd;
        if(Random.Range(0, 100) < weapon.data.crit + weapon.critChanceToAdd) damage = (int)(damage * ((weapon.data
            .critDmg + weapon.critDmgToAdd) / 100f));
        monster.TakeDamage(damage);

        if (monster is AncientSlime { currentState: MonsterState.ATTACK })
        {
            weapon.TakeDamage();
        }
    }
    
    public static void CollisionResolve(BaseWeapon weapon, BaseMonsterProjectile projectile)
    {
        if (!weapon || !projectile) return;

        if (weapon.currentState == WeaponState.NORMAL)
        {
            weapon.TakeDamage();
        }

        if (projectile is LaserBeam)
        {
            return;
        }
        
        projectile.Destroy();
    }

    public static void MonsterDamageWeapon(BaseWeapon weapon, BaseMonster monster)
    {
        if (!weapon || !monster) return;

        if (weapon.currentState == WeaponState.NORMAL)
        {
            weapon.TakeDamage();
            Debug.Log($"<color=red>{monster.name} damaged {weapon.name}</color>");
        }
    }

    public static void WeaponDamageToMonster(BaseWeapon weapon, BaseMonster monster)
    {
        if (!weapon || !monster) return;

        int damage = weapon.data.atk + weapon.damageToAdd;
        if(Random.Range(0, 100) < weapon.data.crit + weapon.critChanceToAdd) damage = (int)(damage * ((weapon.data
            .critDmg + weapon.critDmgToAdd) / 100f));
        monster.TakeDamage(damage);
        Debug.Log($"<color=green>{weapon.name} damaged {monster.name}!</color>");
    }
}
