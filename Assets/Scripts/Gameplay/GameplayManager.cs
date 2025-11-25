using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SingletonDestroy<GameplayManager>
{
    [Header("Controller")] 
    public WeaponController weaponController;
    public MonsterController monsterController;
    
    [Header("Test Level Data")] public WeaponType weaponType;
    
    private void Start()
    {
        weaponController.ChooseWeapon(weaponType);
    }
}
