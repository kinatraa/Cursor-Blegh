using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeResetBuff : BaseBuff
{
    public LifeResetBuff(BuffData data) : base(data)
    {
    }
    
    public override void Init()
    {
        base.Init();
        GameEventManager.onNextWave += Remove;
    }

    public override void Dispose()
    {
        base.Dispose();
        GameEventManager.onNextWave -= Remove;
    }

    public override void AddStack()
    {
        base.AddStack();
        GameEventManager.InvokeAddRerollAmount(3);
    }
}
