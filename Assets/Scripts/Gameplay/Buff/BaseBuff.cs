using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBuff
{
    protected BuffData data;
    
    public BaseBuff(BuffData data)
    {
        this.data = data;
    }
}
