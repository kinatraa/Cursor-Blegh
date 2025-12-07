using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingPopup : UIPopup
{
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    
    private int _musicVolume = 50;
    private int _sfxVolume = 50;
    
    public void Exit()
    {
        AudioManager.Instance.ShotSfx("button_click");
        Hide();
    }

    public override void Show()
    {
        base.Show();
        UpdateText();
        UIManager.Instance.HideUIMenu();
    }

    public override void Hide()
    {
        base.Hide();
        
        UIManager.Instance.ShowUIMenu();
    }

    public void ChangeMusicVolume()
    {
        AudioManager.Instance.ShotSfx("button_click");
        _musicVolume += 10;
        if(_musicVolume > 100) _musicVolume = 0;

        UpdateText();
        
        GameEventManager.InvokeChangeBgmVolume(_musicVolume);
    }
    
    public void ChangeSfxVolume()
    {
        AudioManager.Instance.ShotSfx("button_click");
        _sfxVolume += 10;
        if(_sfxVolume > 100) _sfxVolume = 0;

        UpdateText();
        
        GameEventManager.InvokeChangeSfxVolume(_sfxVolume);
    }

    private void UpdateText()
    {
        musicVolumeText.text = $"{_musicVolume}%";
        sfxVolumeText.text = $"{_sfxVolume}%";
    }
}
