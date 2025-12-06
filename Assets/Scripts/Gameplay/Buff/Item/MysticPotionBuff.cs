using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysticPotionBuff : BaseBuff
{
    public MysticPotionBuff(BuffData data) : base(data)
    {
    }

    public override void Dispose()
    {
        base.Dispose();
        GameplayManager.Instance.monsterController.mysticPotionBuff = null;
    }

    public override void AddStack()
    {
        base.AddStack();
        GameplayManager.Instance.monsterController.mysticPotionBuff = this;
    }
}
