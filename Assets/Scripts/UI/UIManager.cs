using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonDestroy<UIManager>
{
    public UIGameplay uiGameplay;

    private void OnEnable()
    {
        GameEventManager.onUpdatePlayerMaxHP += SetupHealthBar;
        GameEventManager.onUpdatePlayerHP += UpdateHealthBar;
    }

    private void OnDisable()
    {
        GameEventManager.onUpdatePlayerMaxHP -= SetupHealthBar;
        GameEventManager.onUpdatePlayerHP -= UpdateHealthBar;
    }

    private void SetupHealthBar(int maxHp)
    {
        uiGameplay.uiHUD.SetupHealthBar(maxHp);
    }

    private void UpdateHealthBar(int currentHp)
    {
        uiGameplay.uiHUD.UpdateHealthBar(currentHp);
    }
}
