using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;

public class GameplayManager : SingletonDestroy<GameplayManager>
{
    [Header("Controller")] 
    public WeaponController weaponController;
    public MonsterController monsterController;
    public WaveController waveController;
    public StateMachine stateController;
    
    [Header("Test Level Data")] public WeaponType weaponType;
    
    private void Start()
    {
        weaponController.ChooseWeapon(weaponType);
    }
}
