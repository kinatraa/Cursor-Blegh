using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    [Header("Controller")] public WeaponController weaponController;
    public MonsterController monsterController;
    public WaveController waveController;
    public StateMachine stateController;
    public BuffController buffController;
    public WaveRewardSystem waveRewardSystem;

    public WeaponType currentWeaponType = WeaponType.WOODEN_SWORD;
    public int currentWave = 1;

    private void OnEnable()
    {
        GameEventManager.onGameStart += StartGame;
        GameEventManager.onNextWave += NextWave;
    }

    private void OnDisable()
    {
        GameEventManager.onGameStart -= StartGame;
        GameEventManager.onNextWave -= NextWave;
    }

    private void Start()
    {
    }

    private void StartGame()
    {
        ResetGame();

        currentWave = 1;
        waveController.SetCurrentWave(currentWave);

        if (waveRewardSystem != null)
        {
            waveRewardSystem.StartWave();
        }
    }

    private void NextWave()
    {
        currentWave++;
        waveController.SetCurrentWave(currentWave);
        stateController.ChangeState(0);
    }

    private void ResetGame()
    {
        weaponController.Reset();
        monsterController.Reset();
        waveController.Reset();
        stateController.gameObject.SetActive(true);
        stateController.StartMachine();
        buffController.Reset();
        waveRewardSystem.Reset();
    }
}