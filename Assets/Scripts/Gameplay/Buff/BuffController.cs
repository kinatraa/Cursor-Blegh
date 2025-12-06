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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            var newBuff = buffSystem.GetBuff(BuffType.SENTINEL_SHIELD);
            if (newBuff != null && !newBuff.IsMaxStack())
            {
                Debug.Log("Added SENTINEL_SHIELD");
                newBuff.AddStack();
                if(!currentBuffSkills.Contains(newBuff))
                {
                    currentBuffSkills.Add(newBuff);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            var newBuff = buffSystem.GetBuff(BuffType.IMMORTAL);
            if (newBuff != null)
            {
                Debug.Log("Added IMMORTAL");
                newBuff.AddStack();
                if (!currentBuffItems.Contains(newBuff))
                {
                    currentBuffItems.Add(newBuff);
                }
            }
        }
    }

    public void CheckBuffs()
    {
        foreach (var buff in currentBuffSkills)
        {
            buff?.Activate();
        }
        GameplayManager.Instance.monsterController.vampiricRageBuff?.Activate();
    }
}
