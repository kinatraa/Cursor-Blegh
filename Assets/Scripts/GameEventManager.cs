using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager
{
    public static Action onGameStart;
    public static Action onGameWin;
    public static Action onGameLose;
    public static Action onReplayGame;

    public static Action<int> onUpdatePlayerMaxHP;
    public static Action<int> onUpdatePlayerHP;
    
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

    public static void InvokeReplayGame()
    {
        onReplayGame?.Invoke();
    }
    
    public static void InvokeUpdatePlayerMaxHp(int maxHp)
    {
        onUpdatePlayerMaxHP?.Invoke(maxHp);
    }

    public static void InvokeUpdatePlayerHp(int currentHp)
    {
        onUpdatePlayerHP?.Invoke(currentHp);
    }
}
