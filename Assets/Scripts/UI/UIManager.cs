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
        
        LeaderboardController.Instance.RegisterPlayer("Duc Enh");
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
        GameEventManager.onUpdateCombo += UpdateComboText;
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
        GameEventManager.onUpdateCombo -= UpdateComboText;
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

    private void UpdateComboText(int combo)
    {
        uiGameplay.UpdateComboText(combo);
    }

    private void Reset()
    {
        Time.timeScale = 0;
        
        uiGameplay.Reset();
        
        ShowUIMenu();
        HideUIChooseWeapon();
        HideUIGameplay();

        ClearAllProjectiles();
        ClearAllMonsters();
        ClearAllHealthPacks();

        GameplayManager.Instance.weaponController.SetDefaultCursor();
        
        chooseUpgradePopup.gameObject.SetActive(false);
        settingPopup.gameObject.SetActive(false);
        pausePopup.gameObject.SetActive(false);
        losePopup.gameObject.SetActive(false);
        leaderboardPopup.gameObject.SetActive(false);
    }

    private void ClearAllProjectiles()
    {
        BaseMonsterProjectile[] playerProjectiles = FindObjectsOfType<BaseMonsterProjectile>();
        foreach (var projectile in playerProjectiles)
        {
            Destroy(projectile.gameObject);
        }
    }

    private void ClearAllMonsters()
    {
        if (GameplayManager.Instance?.monsterController == null) return;
        
        var monsterController = GameplayManager.Instance.monsterController;
        var monsters = new List<BaseMonster>(monsterController.MonsterList);
        
        foreach (var monster in monsters)
        {
            if (monster != null)
            {
                Destroy(monster.gameObject);
            }
        }
        
        monsterController.ClearAllMonsters();
        
        Debug.Log($"<color=orange>Cleared {monsters.Count} monsters</color>");
    }

    private void ClearAllHealthPacks()
    {
        if (GameplayManager.Instance?.monsterController == null) return;
        
        var monsterController = GameplayManager.Instance.monsterController;
        var healthPacks = new List<GameObject>(monsterController.heartObjects);
        
        foreach (var pack in healthPacks)
        {
            if (pack != null)
            {
                Destroy(pack);
            }
        }
        
        monsterController.heartObjects.Clear();
        
        Debug.Log($"<color=orange>Cleared {healthPacks.Count} health packs</color>");
    }
}
