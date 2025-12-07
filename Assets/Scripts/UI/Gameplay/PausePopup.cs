using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePopup : UIPopup
{
    public override void Show()
    {
        base.Show();
        
        Time.timeScale = 0;
    }

    public void Exit()
    {
        AudioManager.Instance.ShotSfx("button_click");
        GameEventManager.InvokeQuitGame();
    }

    public void Resume()
    {
        AudioManager.Instance.ShotSfx("button_click");
        Time.timeScale = 1;
        
        Hide();
    }
}
