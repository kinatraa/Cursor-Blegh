using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplay : MonoBehaviour
{
    public UIHud playerStatsHUD;

    public void SetupHealthBar(int maxHp)
    {
        (playerStatsHUD as PlayerStatsHUD)?.SetupHealthBar(maxHp);
    }

    public void UpdateHealthBar(int hp)
    {
        (playerStatsHUD as PlayerStatsHUD)?.UpdateHealthBar(hp);
    }

    public void UpdateScoreText(int score)
    {
        (playerStatsHUD as PlayerStatsHUD)?.UpdateScoreText(score);
    }

    public void UpdateWaveText(int wave)
    {
        (playerStatsHUD as PlayerStatsHUD)?.UpdateWaveText(wave);
    }
}
