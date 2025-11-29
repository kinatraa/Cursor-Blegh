using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseUpgradePopup : UIPopup
{
    public override void Hide()
    {
        base.Hide();
        
        GameEventManager.InvokeNextWave();
    }
    
    public void ChooseOption1()
    {
        Debug.Log("ChooseOption1");
        
        Hide();
    }
    
    public void ChooseOption2()
    {
        Debug.Log("ChooseOption2");
        
        Hide();
    }
    
    public void ChooseOption3()
    {
        Debug.Log("ChooseOption3");
        
        Hide();
    }
}
