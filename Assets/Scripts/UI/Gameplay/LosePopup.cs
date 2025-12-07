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
        GameEventManager.InvokeQuitGame();
    }

    private void UpdateFinalText()
    {
        scoreText.text = $"{GameplayManager.Instance.weaponController.currentWeapon.currentScore}";
        waveText.text = $"{GameplayManager.Instance.waveController.currentWave}";
        timeText.text = $"{GameplayManager.Instance.playTimeTracker.playTime}";
    }
}