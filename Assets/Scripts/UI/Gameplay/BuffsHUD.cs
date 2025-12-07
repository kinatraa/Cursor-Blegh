using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffsHUD : UIHud
{
    public List<BuffSlot> buffSkillSlots = new List<BuffSlot>();
    public List<BuffSlot> buffItemSlots = new List<BuffSlot>();

    private void Awake()
    {
        Reset();
    }

    private void OnEnable()
    {
        GameEventManager.onUpdateBuffStack += AddBuff;
    }

    private void OnDisable()
    {
        GameEventManager.onUpdateBuffStack -= AddBuff;
    }
    
    private void AddBuff(BuffData buffData)
    {
        if(buffData == null) return;

        if (buffData.collectType == CollectType.CAN_NOT_COLLECT) return;
        
        List<BuffSlot> targetSlots = buffData.effectType == BuffEffectType.SKILL 
            ? buffSkillSlots 
            : buffItemSlots;
        
        ProcessBuffSlots(targetSlots, buffData);
    }

    private void ProcessBuffSlots(List<BuffSlot> buffSlots, BuffData buffData)
    {
        bool alreadyIn = false;
        
        foreach (BuffSlot buffSlot in buffSlots)
        {
            if (buffSlot.gameObject.activeSelf)
            {
                if (buffSlot.buffData.type == buffData.type)
                {
                    buffSlot.AddStack();
                    alreadyIn = true;
                    break;
                }
            }
        }

        if (alreadyIn) return;
        
        foreach (BuffSlot buffSlot in buffSlots)
        {
            if (!buffSlot.gameObject.activeSelf)
            {
                buffSlot.gameObject.SetActive(true);
                buffSlot.Setup(buffData);
                break;
            }
        }
    }

    public override void Reset()
    {
        base.Reset();
        
        foreach (BuffSlot buffSlot in buffSkillSlots)
        {
            buffSlot.Reset();
        }
        foreach (BuffSlot buffSlot in buffItemSlots)
        {
            buffSlot.Reset();
        }
    }
}