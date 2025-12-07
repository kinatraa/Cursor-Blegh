using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LosePopup : UIPopup
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timeText;

    public override void Show()
    {
        base.Show();

        UpdateFinalText();
    }

    public void Exit()
    {
        AudioManager.Instance.ShotSfx("button_click");
        GameEventManager.InvokeQuitGame();
    }

    private void UpdateFinalText()
    {
        var score = GameplayManager.Instance.weaponController.currentWeapon.currentScore;
        var wave = GameplayManager.Instance.waveController.currentWave;
        var time = (int)GameplayManager.Instance.playTimeTracker.playTime;
        
        LeaderboardController.Instance.SubmitScore(wave, score, time);
        
        scoreText.text = $"{score}";
        waveText.text = $"{wave}";
        timeText.text = $"{time}";
    }
}