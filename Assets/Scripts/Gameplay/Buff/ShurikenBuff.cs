using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenBuff : BaseBuff
{
    public ShurikenBuff(BuffData data) : base(data)
    {
    }

    public override void Activate()
    {
        Debug.Log($"ShurikenBuff.Activate() + {stack}");
    }
}
