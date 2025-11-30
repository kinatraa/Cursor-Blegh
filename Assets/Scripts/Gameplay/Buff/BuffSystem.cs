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
                case BuffType.SENTINEL_SHIELD:
                    _buffLogicDict[type] = new SentinelShieldBuff(data);
                    break;
                case BuffType.TRUE_SHOT:
                    _buffLogicDict[type] = new TrueShotBuff(data);
                    break;
                case BuffType.POISON_THORN:
                    _buffLogicDict[type] = new PoisonThornBuff(data);
                    break;
                case BuffType.THUNDER_STRIKE:
                    _buffLogicDict[type] = new ThunderStrikeBuff(data);
                    break;
                case BuffType.FROST_PRISON:
                    _buffLogicDict[type] = new FrostPrisonBuff(data);
                    break;
                case BuffType.INFERNO_FLAME:
                    _buffLogicDict[type] = new InfernoFlameBuff(data);
                    break;
                case BuffType.MOLTEN_STONE:
                    _buffLogicDict[type] = new MoltenStoneBuff(data);
                    break;
            }
        }

        return _buffLogicDict[type];
    }
}
