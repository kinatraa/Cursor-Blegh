using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseBuff
{
    protected BuffData data;
    protected int stack;
    
    public BaseBuff(BuffData data)
    {
        this.data = data;
        this.stack = 0;
    }

    public virtual void AddStack()
    {
        stack++;
    }

    public virtual void Activate()
    {
        
    }
}
