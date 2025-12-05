using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    [Header("Controller")] 
    public WeaponController weaponController;
    public MonsterController monsterController;
    public WaveController waveController;
    public StateMachine stateController;
    public BuffController buffController;
    
    [Header("Test Level Data")] 
    public WeaponType weaponType;
    public int currentWave = 1;

    private void OnEnable()
    {
        GameEventManager.onNextWave += NextWave;
    }

    private void OnDisable()
    {
        GameEventManager.onNextWave -= NextWave;
    }

    private void Start()
    {
        weaponController.ChooseWeapon(weaponType);
        waveController.SetCurrentWave(currentWave);

        if (WaveRewardSystem.Instance != null)
        {
            WaveRewardSystem.Instance.StartWave();
        }
    }

    private void NextWave()
    {
        currentWave++;
        waveController.SetCurrentWave(currentWave);
        stateController.ChangeState(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            stateController.Next();
        }
    }
}
