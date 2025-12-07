using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public void Play()
    {
        AudioManager.Instance.ShotSfx("button_click");
        UIManager.Instance.ShowUIChooseWeapon();
        UIManager.Instance.HideUIMenu();
    }

    public void Setting()
    {
        AudioManager.Instance.ShotSfx("button_click");
        UIManager.Instance.settingPopup.Show();
    }

    public void Leaderboard()
    {
        AudioManager.Instance.ShotSfx("button_click");
        UIManager.Instance.leaderboardPopup.Show();
    }
}
