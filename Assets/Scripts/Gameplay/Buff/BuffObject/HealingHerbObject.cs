using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealingHerbObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ConstTag.WEAPON))
        {
            var weapon = GameplayManager.Instance.weaponController.currentWeapon;
            weapon.maxHp++;
            GameEventManager.InvokeUpdatePlayerMaxHp(weapon.maxHp);
            weapon.currentHp++;
            GameEventManager.InvokeUpdatePlayerHp(weapon.currentHp);
            
            Destroy(gameObject);
        }
    }
}
