using System;
using UnityEngine;

public class PlayTimeTracker : MonoBehaviour
{
    public float playTime = 0f;
    private bool _run = false;

    private void OnEnable()
    {
        GameEventManager.onGameStart += StartTime;
        GameEventManager.onGameLose += PauseTime;
    }

    private void OnDisable()
    {
        GameEventManager.onGameStart -= StartTime;
        GameEventManager.onGameLose -= PauseTime;
    }

    void Update()
    {
        if (_run)
        {
            playTime += Time.deltaTime;
        }
    }

    private void StartTime()
    {
        playTime = 0f;
        _run = true;
    }

    private void PauseTime()
    {
        _run = false;
    }
}