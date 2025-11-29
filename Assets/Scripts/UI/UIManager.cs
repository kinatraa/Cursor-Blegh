using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonDestroy<UIManager>
{
    public UIGameplay uiGameplay;
    
    [Header("Popup")]
    public UIPopup chooseUpgradePopup;

    private void OnEnable()
    {
        GameEventManager.onChooseUpgradeState += chooseUpgradePopup.Show;
        
        GameEventManager.onUpdatePlayerMaxHP += SetupHealthBar;
        GameEventManager.onUpdatePlayerHP += UpdateHealthBar;
        GameEventManager.onUpdatePlayerScore += UpdateScoreText;
        GameEventManager.onUpdateWave += UpdateWaveText;
    }

    private void OnDisable()
    {
        GameEventManager.onChooseUpgradeState -= chooseUpgradePopup.Show;
        
        GameEventManager.onUpdatePlayerMaxHP -= SetupHealthBar;
        GameEventManager.onUpdatePlayerHP -= UpdateHealthBar;
        GameEventManager.onUpdatePlayerScore -= UpdateScoreText;
        GameEventManager.onUpdateWave -= UpdateWaveText;
    }

    private void SetupHealthBar(int maxHp)
    {
        uiGameplay.SetupHealthBar(maxHp);
    }

    private void UpdateHealthBar(int currentHp)
    {
        uiGameplay.UpdateHealthBar(currentHp);
    }

    private void UpdateScoreText(int score)
    {
        uiGameplay.UpdateScoreText(score);
    }

    private void UpdateWaveText(int wave)
    {
        uiGameplay.UpdateWaveText(wave);
    }
}
