using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public List<BaseWeapon> weapons = new List<BaseWeapon>();
    public BaseWeapon currentWeapon;
    public RebornBuff rebornBuff = null;
    
    public WeaponSkillSystem weaponSkillSystem;
    
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        
        var allSkills = Resources.LoadAll<WeaponSkillData>("WeaponData/WeaponSkillData");
        weaponSkillSystem = new WeaponSkillSystem(new List<WeaponSkillData>(allSkills));
    }

    private void Update()
    {
        if (currentWeapon)
        {
            var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            currentWeapon.transform.position = mousePos;
        }

        if (Input.GetMouseButtonDown(1))
        {
            var skill = weaponSkillSystem.GetSkill(currentWeapon.data.skill);
            if (skill != null)
            {
                if (!skill.IsOnCooldown())
                {
                    skill.Activate(currentWeapon);
                }
                else
                {
                    Debug.Log($"<color=red>Skill on cooldown: {skill.GetRemainingCooldown():F1}s</color>");
                }
            }
        }
    }

    public void ChooseWeapon(WeaponType type)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.data.weaponType == type)
            {
                weapon.gameObject.SetActive(true);
                weapon.ResetWeapon();
                currentWeapon = weapon;
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }
}
