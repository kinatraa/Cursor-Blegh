using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public UIGameplay uiGameplay;
    public UIMenu uiMenu;
    public UIChooseWeapon uiChooseWeapon;
    
    [Header("Popup")]
    public UIPopup chooseUpgradePopup;

    private void Start()
    {
        ShowUIMenu();
        HideUIChooseWeapon();
        HideUIGameplay();
    }

    private void OnEnable()
    {
        GameEventManager.onGameStart += ShowUIGameplay;
        
        GameEventManager.onChooseUpgradeState += chooseUpgradePopup.Show;
        
        GameEventManager.onUpdatePlayerMaxHP += SetupHealthBar;
        GameEventManager.onUpdatePlayerHP += UpdateHealthBar;
        GameEventManager.onUpdatePlayerScore += UpdateScoreText;
        GameEventManager.onUpdateWave += UpdateWaveText;
    }

    private void OnDisable()
    {
        GameEventManager.onGameStart -= ShowUIGameplay;
        
        GameEventManager.onChooseUpgradeState -= chooseUpgradePopup.Show;
        
        GameEventManager.onUpdatePlayerMaxHP -= SetupHealthBar;
        GameEventManager.onUpdatePlayerHP -= UpdateHealthBar;
        GameEventManager.onUpdatePlayerScore -= UpdateScoreText;
        GameEventManager.onUpdateWave -= UpdateWaveText;
    }

    public void ShowUIMenu()
    {
        uiMenu.gameObject.SetActive(true);
    }

    public void HideUIMenu()
    {
        uiMenu.gameObject.SetActive(false);
    }

    public void ShowUIChooseWeapon()
    {
        uiChooseWeapon.gameObject.SetActive(true);
    }

    public void HideUIChooseWeapon()
    {
        uiChooseWeapon.gameObject.SetActive(false);
    }

    public void ShowUIGameplay()
    {
        uiGameplay.gameObject.SetActive(true);
    }
    
    public void HideUIGameplay()
    {
        uiGameplay.gameObject.SetActive(false);
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
