using System.Collections.Generic;
using UnityEngine;

public static class CombatResolver
{
    public static void CollisionResolve(BaseWeapon weapon, BaseMonster monster)
    {
        if (!weapon || !monster) return;
        if (weapon.currentState == WeaponState.BLINK) return;

        // Sound
        List<string> hitSounds = new List<string> 
            { "attack_1", "attack_2", "weapon_swish", "weapon_swoosh" };

        string hitKey = hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)];
        AudioManager.Instance?.ShotSfx(hitKey, volume: 0.75f);

        // Slash
        weapon.PlaySlash();

        // Damage
        int damage = weapon.data.atk + weapon.damageToAdd;

        if (Random.Range(0, 100) < weapon.data.crit + weapon.critChanceToAdd)
            damage = (int)(damage * ((weapon.data.critDmg + weapon.critDmgToAdd) / 100f));

        monster.TakeDamage(damage);

        if (monster is AncientSlime { currentState: MonsterState.ATTACK })
            weapon.TakeDamage();
    }

    public static void CollisionResolve(BaseWeapon weapon, BaseMonsterProjectile projectile)
    {
        if (!weapon || !projectile) return;

        if (weapon.currentState == WeaponState.NORMAL)
            weapon.TakeDamage();

        if (projectile is LaserBeam) return;
        projectile.Destroy();
    }

    public static void MonsterDamageWeapon(BaseWeapon weapon, BaseMonster monster)
    {
        if (!weapon || !monster) return;

        if (weapon.currentState == WeaponState.NORMAL)
            weapon.TakeDamage();
    }

    public static void WeaponDamageToMonster(BaseWeapon weapon, BaseMonster monster)
    {
        if (!weapon || !monster) return;

        weapon.PlaySlash();

        int damage = weapon.data.atk + weapon.damageToAdd;

        if (Random.Range(0, 100) < weapon.data.crit + weapon.critChanceToAdd)
            damage = (int)(damage * ((weapon.data.critDmg + weapon.critDmgToAdd) / 100f));

        monster.TakeDamage(damage);
    }
}