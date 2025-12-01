using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    public List<BaseBuff> currentBuffs = new List<BaseBuff>();
    
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
            var newBuff = buffSystem.GetBuff(BuffType.SHURIKEN);
            if (newBuff != null)
            {
                Debug.Log("Added SHURIKEN");
                newBuff.AddStack();
                if(!currentBuffs.Contains(newBuff))
                {
                    currentBuffs.Add(newBuff);
                }
            }
        }
    }

    public void CheckBuffs()
    {
        foreach (var buff in currentBuffs)
        {
            buff?.Activate();
        }
    }
}
