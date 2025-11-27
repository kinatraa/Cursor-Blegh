using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatResolver
{
    public static void CollisionResolve(BaseWeapon weapon, BaseMonster monster)
    {
        if (!weapon || !monster) return;

        if (monster.onAttack)
        {
            weapon.TakeDamage();
        }
        else
        {
            int damage = weapon.data.atk;
            if(Random.Range(0, 100) < weapon.data.crit) damage = (int)(damage * (weapon.data.critDmg / 100f));
            monster.TakeDamage(damage);
        }
    }
}
