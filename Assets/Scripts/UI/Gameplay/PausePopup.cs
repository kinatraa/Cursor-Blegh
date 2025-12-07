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
        GameEventManager.InvokeQuitGame();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        
        Hide();
    }
}
