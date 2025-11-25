using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public WeaponType weaponType;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ConstTag.MONSTER))
        {
            Debug.Log($"Hit {other.name}");
        }
    }
}

public enum WeaponType
{
    NONE = 0,
    WOODEN_SWORD = 1,
    DAGGER = 2,
    BATTLE_AXE = 3,
    PREMIUM_SWORD = 4,
}
