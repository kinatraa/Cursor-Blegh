using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBuff
{
    public readonly BuffData data;
    protected int stack;
    public int GetStack => stack;
    
    public BaseBuff(BuffData data)
    {
        this.data = data;
        this.stack = 0;

        Init();
    }

    public virtual void Init()
    {
        
    }
    
    public virtual void Dispose()
    {
        
    }

    public virtual void AddStack()
    {
        stack++;
    }

    public virtual void Activate()
    {
        
    }

    public virtual void Remove()
    {
        var buffController = GameplayManager.Instance.buffController;
        if (data.effectType == BuffEffectType.SKILL)
        {
            Dispose();
            buffController.currentBuffSkills.Remove(this);
        }
        else if (data.effectType == BuffEffectType.ITEM)
        {
            Dispose();
            buffController.currentBuffItems.Remove(this);
        }
    }

    public virtual bool IsMaxStack()
    {
        if (data.maxStack == 0) return false;
        return stack >= data.maxStack;
    }
    
    public BuffType GetBuffType() => data.type;
    
    protected Coroutine StartCoroutine(IEnumerator r) 
        => CoroutineRunner.Instance.StartCoroutine(r);

    protected void StopCoroutine(Coroutine r) 
        => CoroutineRunner.Instance.StopCoroutine(r);
}
