using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebornBuff : BaseBuff
{
    private bool hasRevived = false;
    public RebornBuff(BuffData data) : base(data)
    {
    }
    public override void Dispose()
    {
        base.Dispose();
        if (GameplayManager.Instance?.weaponController != null)
        {
            GameplayManager.Instance.weaponController.rebornBuff = null;
        }
    }

    public override void AddStack()
    {
        base.AddStack();
        hasRevived = false;
        GameplayManager.Instance.weaponController.rebornBuff = this;
    }

    public bool TryRevive()
    {
        if (!hasRevived)
        {
            string hitKey = "item_pickup";
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ShotSfx(hitKey, pitch: 2f);
            }
            hasRevived = true;
            Debug.Log("<color=yellow>IMMORTAL BUFF ACTIVATED - REVIVED WITH 1 HP!</color>");
            return true;
        }

        return false;
    }
}
