using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    protected Coroutine StartCoroutine(IEnumerator r) 
        => CoroutineRunner.Instance.StartCoroutine(r);

}
