using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public void Play()
    {
        UIManager.Instance.ShowUIChooseWeapon();
        UIManager.Instance.HideUIMenu();
    }

    public void Setting()
    {
        UIManager.Instance.settingPopup.Show();
    }

    public void Leaderboard()
    {
        
    }
}
