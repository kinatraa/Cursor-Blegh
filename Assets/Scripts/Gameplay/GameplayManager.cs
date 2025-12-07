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
    public PlayTimeTracker playTimeTracker;

    public WeaponType currentWeaponType = WeaponType.WOODEN_SWORD;
    public int currentWave = 1;

    private bool _loseGame = false;
    
    private void OnEnable()
    {
        GameEventManager.onGameStart += StartGame;
        GameEventManager.onGameLose += LoseGame;
        GameEventManager.onQuitGame += ResetGame;
        GameEventManager.onNextWave += NextWave;
    }

    private void OnDisable()
    {
        GameEventManager.onGameStart -= StartGame;
        GameEventManager.onGameLose -= LoseGame;
        GameEventManager.onQuitGame -= ResetGame;
        GameEventManager.onNextWave -= NextWave;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_loseGame)
        {
            UIManager.Instance.pausePopup.Show();
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1;
        
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

    private void LoseGame()
    {
        _loseGame = true;
        Time.timeScale = 0;
    }

    private void ResetGame()
    {
        _loseGame = false;
        
        weaponController.Reset();
        monsterController.Reset();
        waveController.Reset();
        stateController.gameObject.SetActive(true);
        stateController.StartMachine();
        buffController.Reset();
        waveRewardSystem.Reset();
    }
}