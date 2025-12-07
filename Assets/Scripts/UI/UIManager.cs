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
    public UIPopup settingPopup;
    public UIPopup pausePopup;
    public UIPopup losePopup;
    public UIPopup leaderboardPopup;

    protected override void Awake()
    {
        base.Awake();
        
        Cursor.visible = false;
    }

    private void Start()
    {
        Reset();
    }

    private void OnEnable()
    {
        GameEventManager.onGameStart += ShowUIGameplay;
        GameEventManager.onGameLose += ShowPopupLose;
        GameEventManager.onQuitGame += Reset;
        
        GameEventManager.onChooseUpgradeState += chooseUpgradePopup.Show;
        
        GameEventManager.onUpdatePlayerMaxHP += SetupHealthBar;
        GameEventManager.onUpdatePlayerHP += UpdateHealthBar;
        GameEventManager.onUpdatePlayerScore += UpdateScoreText;
        GameEventManager.onUpdateWave += UpdateWaveText;
    }

    private void OnDisable()
    {
        GameEventManager.onGameStart -= ShowUIGameplay;
        GameEventManager.onGameLose -= ShowPopupLose;
        GameEventManager.onQuitGame -= Reset;
        
        GameEventManager.onChooseUpgradeState -= chooseUpgradePopup.Show;
        
        GameEventManager.onUpdatePlayerMaxHP -= SetupHealthBar;
        GameEventManager.onUpdatePlayerHP -= UpdateHealthBar;
        GameEventManager.onUpdatePlayerScore -= UpdateScoreText;
        GameEventManager.onUpdateWave -= UpdateWaveText;
    }

    public void ShowPopupLose()
    {
        losePopup.Show();
    }

    public void HidePopupLose()
    {
        losePopup.Hide();
    }

    public void ShowUIMenu()
    {
        AudioManager.Instance.PlayMusic("bgm_mainmenu");
        uiMenu.gameObject.SetActive(true);
    }

    public void HideUIMenu()
    {
        uiMenu.gameObject.SetActive(false);
    }

    public void ShowUIChooseWeapon()
    {
        AudioManager.Instance.PlayMusic("bgm_gameplay");
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

    private void Reset()
    {
        Time.timeScale = 0;
        
        uiGameplay.Reset();
        GameplayManager.Instance.weaponController.SetDefaultCursor();
        
        ShowUIMenu();
        HideUIChooseWeapon();
        HideUIGameplay();
        
        chooseUpgradePopup.gameObject.SetActive(false);
        settingPopup.gameObject.SetActive(false);
        pausePopup.gameObject.SetActive(false);
        losePopup.gameObject.SetActive(false);
        leaderboardPopup.gameObject.SetActive(false);
    }
}
