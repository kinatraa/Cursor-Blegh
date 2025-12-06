using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    public List<BaseBuff> currentBuffSkills = new List<BaseBuff>();
    public List<BaseBuff> currentBuffItems  = new List<BaseBuff>();
    
    public BuffSystem buffSystem;

    private void Awake()
    {
        var allBuffs = Resources.LoadAll<BuffData>("BuffData");
        buffSystem = new BuffSystem(new List<BuffData>(allBuffs));
    }

    private void OnEnable()
    {
        GameEventManager.onChooseBuff += AddBuff;
    }

    private void OnDisable()
    {
        GameEventManager.onChooseBuff -= AddBuff;
    }

    private void AddBuff(BuffData buffData)
    {
        var newBuff = buffSystem.GetBuff(buffData.type);
        if (newBuff == null) return;

        newBuff.AddStack();
        switch (newBuff.data.effectType)
        {
            case BuffEffectType.SKILL:
                if(!currentBuffSkills.Contains(newBuff))
                {
                    currentBuffSkills.Add(newBuff);
                }
                break;
            case BuffEffectType.ITEM:
                if (!currentBuffItems.Contains(newBuff))
                {
                    currentBuffItems.Add(newBuff);
                }
                break;
        }
        GameEventManager.InvokeUpdateBuffStack(buffData);
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.J))
    //     {
    //         var newBuff = buffSystem.GetBuff(BuffType.MOLTEN_STONE);
    //         if (newBuff != null && !newBuff.IsMaxStack())
    //         {
    //             Debug.Log("Added MOLTEN_STONE");
    //             newBuff.AddStack();
    //             if(!currentBuffSkills.Contains(newBuff))
    //             {
    //                 currentBuffSkills.Add(newBuff);
    //             }
    //         }
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.K))
    //     {
    //         var newBuff = buffSystem.GetBuff(BuffType.IMMORTAL);
    //         if (newBuff != null)
    //         {
    //             Debug.Log("Added IMMORTAL");
    //             newBuff.AddStack();
    //             if (!currentBuffItems.Contains(newBuff))
    //             {
    //                 currentBuffItems.Add(newBuff);
    //             }
    //         }
    //     }
    // }

    public void CheckBuffs()
    {
        foreach (var buff in currentBuffSkills)
        {
            buff?.Activate();
        }
        GameplayManager.Instance.monsterController.vampiricRageBuff?.Activate();
    }

    public bool IsBuffMaxStack(BuffType buffType)
    {
        foreach (var buff in currentBuffItems)
        {
            if (buff.data.type == buffType)
            {
                return buff.IsMaxStack();
            }
        }
        
        foreach (var buff in currentBuffSkills)
        {
            if (buff.data.type == buffType)
            {
                return buff.IsMaxStack();
            }
        }

        return false;
    }

    public int GetBuffCurrentStack(BuffType buffType)
    {
        foreach (var buff in currentBuffItems)
        {
            if (buff.data.type == buffType)
            {
                return buff.GetStack;
            }
        }
        
        foreach (var buff in currentBuffSkills)
        {
            if (buff.data.type == buffType)
            {
                return buff.GetStack;
            }
        }

        return 0;
    }

    public void Reset()
    {
        currentBuffSkills = new List<BaseBuff>();
        currentBuffItems = new List<BaseBuff>();
    }
}
