using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventManager
{
    public static Action onGameStart;
    public static Action onGameWin;
    public static Action onGameLose;
    public static Action onQuitGame;

    public static Action onNextWave;
    
    public static Action<int> onChangeBgmVolume;
    public static Action<int> onChangeSfxVolume;
    
    // State
    public static Action onChooseUpgradeState;

    // Update
    public static Action<int> onUpdatePlayerMaxHP;
    public static Action<int> onUpdatePlayerHP;
    public static Action<int> onUpdatePlayerScore;
    public static Action<int> onUpdateWave;
    public static Action<int> onAddRerollAmount;
    public static Action<BuffData> onChooseBuff;
    public static Action<BuffData> onUpdateBuffStack;
    public static Action<WeaponType> onChooseWeapon;
    
    public static void InvokeGameStart()
    {
        onGameStart?.Invoke();
    }

    public static void InvokeGameWin()
    {
        onGameWin?.Invoke();
    }

    public static void InvokeGameLose()
    {
        onGameLose?.Invoke();
    }

    public static void InvokeQuitGame()
    {
        onQuitGame?.Invoke();
    }

    public static void InvokeNextWave()
    {
        onNextWave?.Invoke();
    }

    public static void InvokeChooseUpgradeState()
    {
        onChooseUpgradeState?.Invoke();
    }
    
    public static void InvokeUpdatePlayerMaxHp(int maxHp)
    {
        onUpdatePlayerMaxHP?.Invoke(maxHp);
    }

    public static void InvokeUpdatePlayerHp(int currentHp)
    {
        onUpdatePlayerHP?.Invoke(currentHp);
    }
    
    public static void InvokeUpdatePlayerScore(int score)
    {
        onUpdatePlayerScore?.Invoke(score);
    }

    public static void InvokeUpdateWave(int wave)
    {
        onUpdateWave?.Invoke(wave);
    }

    public static void InvokeAddRerollAmount(int amount)
    {
        onAddRerollAmount?.Invoke(amount);
    }

    public static void InvokeChooseBuff(BuffData buffData)
    {
        onChooseBuff?.Invoke(buffData);
    }

    public static void InvokeUpdateBuffStack(BuffData buffData)
    {
        onUpdateBuffStack?.Invoke(buffData);
    }

    public static void InvokeChooseWeapon(WeaponType weaponType)
    {
        onChooseWeapon?.Invoke(weaponType);
    }

    public static void InvokeChangeBgmVolume(int newVolume)
    {
        onChangeBgmVolume?.Invoke(newVolume);
    }

    public static void InvokeChangeSfxVolume(int newVolume)
    {
        onChangeSfxVolume?.Invoke(newVolume);
    }
}
