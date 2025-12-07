using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingPopup : UIPopup
{
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    
    private int _musicVolume = 100;
    private int _sfxVolume = 100;
    
    public void Exit()
    {
        Hide();
    }

    public override void Show()
    {
        base.Show();
        
        UIManager.Instance.HideUIMenu();
    }

    public override void Hide()
    {
        base.Hide();
        
        UIManager.Instance.ShowUIMenu();
    }

    public void ChangeMusicVolume()
    {
        _musicVolume += 10;
        if(_musicVolume > 100) _musicVolume = 0;

        musicVolumeText.text = $"{_musicVolume}%";
        
        GameEventManager.InvokeChangeBgmVolume(_musicVolume);
    }
    
    public void ChangeSfxVolume()
    {
        _sfxVolume += 10;
        if(_sfxVolume > 100) _sfxVolume = 0;
        
        sfxVolumeText.text = $"{_sfxVolume}%";
        
        GameEventManager.InvokeChangeSfxVolume(_sfxVolume);
    }
}
