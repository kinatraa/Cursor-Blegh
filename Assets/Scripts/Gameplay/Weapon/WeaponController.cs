using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public List<BaseWeapon> weapons = new List<BaseWeapon>();
    public BaseWeapon currentWeapon;
    
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (currentWeapon)
        {
            var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            currentWeapon.transform.position = mousePos;
        }
    }

    public void ChooseWeapon(WeaponType type)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.weaponType == type)
            {
                weapon.gameObject.SetActive(true);
                currentWeapon = weapon;
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }
}
