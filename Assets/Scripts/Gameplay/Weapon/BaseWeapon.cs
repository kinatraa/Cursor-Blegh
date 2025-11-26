using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public WeaponData data;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ConstTag.MONSTER))
        {
            // Debug.Log($"Hit {other.name}");
        }
    }
}
