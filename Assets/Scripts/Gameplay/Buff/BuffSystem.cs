using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem
{
    private Dictionary<BuffType, BuffData> _buffDataDict;
    private Dictionary<BuffType, BaseBuff> _buffLogicDict;

    public BuffSystem(List<BuffData> allBuffs)
    {
        _buffDataDict = new Dictionary<BuffType, BuffData>();

        foreach (var s in allBuffs)
        {
            _buffDataDict[s.type] = s;
        }

        _buffLogicDict = new Dictionary<BuffType, BaseBuff>();
    }

    public BaseBuff GetBuff(BuffType type)
    {
        if (!_buffLogicDict.ContainsKey(type))
        {
            var data = _buffDataDict[type];

            switch (type)
            {
                case BuffType.SHURIKEN:
                    _buffLogicDict[type] = new ShurikenBuff(data);
                    break;
            }
        }

        return _buffLogicDict[type];
    }
}
