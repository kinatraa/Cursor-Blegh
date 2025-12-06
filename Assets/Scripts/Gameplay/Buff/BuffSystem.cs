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

    public BuffData GetBuffData(BuffType buffType)
    {
        if (_buffDataDict.ContainsKey(buffType))
        {
            return _buffDataDict[buffType];
        }
        return null;
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
                case BuffType.LIFE_RESET:
                    _buffLogicDict[type] = new LifeResetBuff(data);
                    break;
                case BuffType.VAMPIRIC_RAGE:
                    _buffLogicDict[type] = new VampiricRageBuff(data);
                    break;
                case BuffType.IMMORTAL:
                    _buffLogicDict[type] = new ImmortalBuff(data);
                    break;
                case BuffType.HEALING_HERB:
                    _buffLogicDict[type] = new HealingHerbBuff(data);
                    break;
                case BuffType.REBORN:
                    _buffLogicDict[type] = new RebornBuff(data);
                    break;
                case BuffType.THICK_CLOTH:
                    _buffLogicDict[type] = new ThickClothBuff(data);
                    break;
                case BuffType.PRISM_LENS:
                    _buffLogicDict[type] = new PrismLensBuff(data);
                    break;
                case BuffType.ARCANE_TOME:
                    _buffLogicDict[type] = new ArcaneTomeBuff(data);
                    break;
                case BuffType.SHARPEN_STONE:
                    _buffLogicDict[type] = new SharpenStoneBuff(data);
                    break;
                case BuffType.MEDIC_BANDAGE:
                    _buffLogicDict[type] = new MedicBandageBuff(data);
                    break;
                case BuffType.FAIRY_ELIXIR:
                    _buffLogicDict[type] = new FairyElixirBuff(data);
                    break;
                case BuffType.MYSTIC_POTION:
                    _buffLogicDict[type] = new MysticPotionBuff(data);
                    break;
            }
        }

        return _buffLogicDict[type];
    }
}
