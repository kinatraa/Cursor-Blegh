using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplay : MonoBehaviour
{
    public UIHud playerStatsHUD;
    public UIHud buffsHUD;

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
    
    public void UpdateComboText(int combo)
    {
        (playerStatsHUD as PlayerStatsHUD)?.UpdateComboText(combo);
    }

    public void Reset()
    {
        playerStatsHUD.Reset();
        buffsHUD.Reset();
    }
}
