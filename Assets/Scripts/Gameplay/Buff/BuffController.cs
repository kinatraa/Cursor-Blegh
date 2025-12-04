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
            var newBuff = buffSystem.GetBuff(BuffType.MOLTEN_STONE);
            if (newBuff != null && !newBuff.IsMaxStack())
            {
                Debug.Log("Added MOLTEN_STONE");
                newBuff.AddStack();
                if(!currentBuffSkills.Contains(newBuff))
                {
                    currentBuffSkills.Add(newBuff);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            var newBuff = buffSystem.GetBuff(BuffType.THICK_CLOTH);
            if (newBuff != null)
            {
                Debug.Log("Added THICK_CLOTH");
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
    }
}
