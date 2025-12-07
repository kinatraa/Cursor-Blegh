using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIChooseWeapon : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    
    public void ChooseWoodenSword()
    {
        AudioManager.Instance.ShotSfx("button_click");
        GameEventManager.InvokeChooseWeapon(WeaponType.WOODEN_SWORD);
        HideUI();
    }

    public void ChooseDagger()
    {
        AudioManager.Instance.ShotSfx("button_click");
        GameEventManager.InvokeChooseWeapon(WeaponType.DAGGER);
        HideUI();
    }

    public void ChooseBlade()
    {
        AudioManager.Instance.ShotSfx("button_click");
        GameEventManager.InvokeChooseWeapon(WeaponType.BATTLE_AXE);
        HideUI();
    }
    
    public void ChooseCrystalSword()
    {
        AudioManager.Instance.ShotSfx("button_click");
        GameEventManager.InvokeChooseWeapon(WeaponType.PREMIUM_SWORD);
        HideUI();
    }

    public void Exit()
    {
        AudioManager.Instance.ShotSfx("button_click");
        UIManager.Instance.ShowUIMenu();
        UIManager.Instance.HideUIChooseWeapon();
    }

    private void HideUI()
    {
        GameEventManager.InvokeGameStart();
        
        gameObject.SetActive(false);
    }

    public void ShowDescription(WeaponData data)
    {
        titleText.text = $"{data.weaponName}";
        descriptionText.text = $"{data.description}";
        titleText.transform.parent.gameObject.SetActive(true);
    }
    
    public void HideDescription()
    {
        titleText.transform.parent.gameObject.SetActive(false);
    }
}
