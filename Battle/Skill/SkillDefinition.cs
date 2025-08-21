using System;
using System.Collections.Generic;
using Server.Battle.Config;

namespace Server.Battle.Skill
{
    #region 技能定义相关枚举和接口

    /// <summary>
    /// 技能类型
    /// </summary>
    public enum SkillType
    {
        Active = 1,     // 主动技能
        Passive = 2,    // 被动技能
        Trigger = 3,    // 触发技能
        Combo = 4       // 连击技能
    }

    /// <summary>
    /// 效果类型
    /// </summary>
    public enum EffectType
    {
        Damage = 1,           // 伤害
        Heal = 2,            // 治疗
        Buff = 3,            // 增益
        Debuff = 4,          // 减益
        Shield = 5,          // 护盾
        Dispel = 6,          // 驱散
        Steal = 7,           // 偷取
        Transform = 8,       // 变形
        Summon = 9,          // 召唤
        Special = 10         // 特殊效果
    }

    /// <summary>
    /// 触发时机
    /// </summary>
    public enum TriggerTiming
    {
        Immediate = 1,       // 立即
        BeforeAction = 2,    // 行动前
        AfterAction = 3,     // 行动后
        OnDamage = 4,        // 受到伤害时
        OnHeal = 5,          // 受到治疗时
        OnDeath = 6,         // 死亡时
        OnKill = 7,          // 击杀时
        OnTurnStart = 8,     // 回合开始
        OnTurnEnd = 9,       // 回合结束
        OnBuffApply = 10,    // Buff生效时
        OnBuffExpire = 11    // Buff失效时
    }

    #endregion

    #region 核心数据结构

    /// <summary>
    /// 技能定义
    /// </summary>
    [Serializable]
    public class SkillDefinition
    {
        public int skillId;                          // 技能ID
        public string skillName;                     // 技能名称
        public SkillType skillType;                  // 技能类型
        public int cooldown;                         // 冷却时间
        public int cost;                            // 消耗（能量/法力等）
        public string description;                   // 技能描述
        public string iconPath;                      // 图标路径
        public string animationName;                 // 动画名称
        public float castTime;                       // 施法时间
        
        public List<SkillEffect> effects;            // 技能效果列表
        public List<SkillCondition> conditions;     // 释放条件
        
        public SkillDefinition()
        {
            effects = new List<SkillEffect>();
            conditions = new List<SkillCondition>();
        }
    }

    /// <summary>
    /// 技能效果
    /// </summary>
    [Serializable]
    public class SkillEffect
    {
        public int effectId;                         // 效果ID
        public EffectType effectType;               // 效果类型
        public TargetType targetType;     // 目标选择
        public TriggerTiming triggerTiming;         // 触发时机
        
        // 数值相关
        public int baseValue;                        // 基础数值
        public float scaleValue;                     // 缩放数值（基于攻击力等）
        public string scaleAttribute;               // 缩放属性（"attack", "defense", "maxHp"等）
        
        // 持续相关
        public int duration;                         // 持续回合数（0表示瞬时）
        public int tickInterval;                     // 触发间隔（回合）
        public bool canStack;                        // 是否可以叠加
        public int maxStacks;                        // 最大叠加层数
        
        // 概率相关
        public float probability;                    // 触发概率（0-1）
        public bool ignoreDefense;                   // 是否无视防御
        public bool canCrit;                         // 是否可以暴击
        
        // 额外参数
        public Dictionary<string, object> parameters; // 自定义参数
        
        public SkillEffect()
        {
            parameters = new Dictionary<string, object>();
            probability = 1.0f; // 默认100%触发
        }
    }

    /// <summary>
    /// 技能释放条件
    /// </summary>
    [Serializable]
    public class SkillCondition
    {
        public string conditionType;                 // 条件类型
        public string targetAttribute;              // 目标属性
        public string comparison;                    // 比较操作符（">", "<", "==", "!="等）
        public float value;                          // 比较值
        public string description;                   // 条件描述
        
        // 常见条件示例：
        // {"conditionType": "SelfHp", "comparison": "<", "value": 0.5} // 自身血量低于50%
        // {"conditionType": "EnemyCount", "comparison": ">=", "value": 2} // 敌人数量>=2
        // {"conditionType": "BuffPresent", "targetAttribute": "poison", "comparison": "==", "value": 1} // 目标有毒状态
    }

    #endregion

    #region Buff系统

    /// <summary>
    /// Buff状态
    /// </summary>
    [Serializable]
    public class BuffState
    {
        public int buffId;                          // BuffID
        public string buffName;                     // Buff名称
        public EffectType buffType;                 // Buff类型
        public long sourceUnitId;                    // 来源单位ID
        public long targetUnitId;                    // 目标单位ID
        public int sourceSkillId;                   // 来源技能ID
        
        public int remainingDuration;               // 剩余持续时间
        public int currentStacks;                   // 当前叠加层数
        public int tickCounter;                     // 跳计数器
        
        public SkillEffect effect;                  // 关联的技能效果
        public Dictionary<string, float> attributeModifiers; // 属性修正器
        
        public DateTime createdTime;                // 创建时间
        public DateTime lastTickTime;               // 上次触发时间
        
        public BuffState()
        {
            attributeModifiers = new Dictionary<string, float>();
            createdTime = DateTime.Now;
            lastTickTime = DateTime.Now;
        }
        
        /// <summary>
        /// 是否已过期
        /// </summary>
        public bool IsExpired => remainingDuration <= 0;
        
        /// <summary>
        /// 是否应该触发
        /// </summary>
        public bool ShouldTick(int currentRound)
        {
            if (effect.tickInterval <= 0) return false;
            return (currentRound - tickCounter) >= effect.tickInterval;
        }
    }

    /// <summary>
    /// Buff管理器
    /// </summary>
    public class BuffManager
    {
        private Dictionary<long, List<BuffState>> _unitBuffs; // 单位ID -> Buff列表
        private int _nextBuffId = 1;
        
        public BuffManager()
        {
            _unitBuffs = new Dictionary<long, List<BuffState>>();
        }
        
        /// <summary>
        /// 添加Buff
        /// </summary>
        public BuffState AddBuff(long targetUnitId, SkillEffect effect, long sourceUnitId, int sourceSkillId)
        {
            if (!_unitBuffs.ContainsKey(targetUnitId))
            {
                _unitBuffs[targetUnitId] = new List<BuffState>();
            }
            
            var buffList = _unitBuffs[targetUnitId];
            
            // 检查是否可以叠加
            if (effect.canStack)
            {
                var existingBuff = buffList.Find(b => b.sourceSkillId == sourceSkillId && b.effect.effectType == effect.effectType);
                if (existingBuff != null)
                {
                    // 叠加现有Buff
                    existingBuff.currentStacks = Math.Min(existingBuff.currentStacks + 1, effect.maxStacks);
                    existingBuff.remainingDuration = effect.duration; // 刷新持续时间
                    return existingBuff;
                }
            }
            
            // 创建新Buff
            var newBuff = new BuffState
            {
                buffId = _nextBuffId++,
                buffName = $"Skill_{sourceSkillId}_Effect_{effect.effectId}",
                buffType = effect.effectType,
                sourceUnitId = sourceUnitId,
                targetUnitId = targetUnitId,
                sourceSkillId = sourceSkillId,
                remainingDuration = effect.duration,
                currentStacks = 1,
                effect = effect
            };
            
            // 计算属性修正器
            CalculateAttributeModifiers(newBuff);
            
            buffList.Add(newBuff);
            return newBuff;
        }
        
        /// <summary>
        /// 移除Buff
        /// </summary>
        public bool RemoveBuff(long unitId, int buffId)
        {
            if (!_unitBuffs.ContainsKey(unitId)) return false;
            
            var buffList = _unitBuffs[unitId];
            return buffList.RemoveAll(b => b.buffId == buffId) > 0;
        }
        
        /// <summary>
        /// 获取单位的所有Buff
        /// </summary>
        public List<BuffState> GetUnitBuffs(long unitId)
        {
            return _unitBuffs.ContainsKey(unitId) ? _unitBuffs[unitId] : new List<BuffState>();
        }
        
        /// <summary>
        /// 更新Buff（每回合调用）
        /// </summary>
        public List<BuffState> UpdateBuffs(long unitId, int currentRound)
        {
            var expiredBuffs = new List<BuffState>();
            
            if (!_unitBuffs.ContainsKey(unitId)) return expiredBuffs;
            
            var buffList = _unitBuffs[unitId];
            for (int i = buffList.Count - 1; i >= 0; i--)
            {
                var buff = buffList[i];
                
                // 减少持续时间
                if (buff.remainingDuration > 0)
                {
                    buff.remainingDuration--;
                }
                
                // 检查是否过期
                if (buff.IsExpired)
                {
                    expiredBuffs.Add(buff);
                    buffList.RemoveAt(i);
                }
            }
            
            return expiredBuffs;
        }
        
        /// <summary>
        /// 计算属性修正器
        /// </summary>
        private void CalculateAttributeModifiers(BuffState buff)
        {
            var effect = buff.effect;
            
            switch (effect.effectType)
            {
                case EffectType.Buff:
                    // 根据参数设置属性修正
                    if (effect.parameters.ContainsKey("attackBonus"))
                    {
                        buff.attributeModifiers["attack"] = Convert.ToSingle(effect.parameters["attackBonus"]);
                    }
                    if (effect.parameters.ContainsKey("defenseBonus"))
                    {
                        buff.attributeModifiers["defense"] = Convert.ToSingle(effect.parameters["defenseBonus"]);
                    }
                    if (effect.parameters.ContainsKey("speedBonus"))
                    {
                        buff.attributeModifiers["speed"] = Convert.ToSingle(effect.parameters["speedBonus"]);
                    }
                    break;
                    
                case EffectType.Debuff:
                    // 减益效果，使用负值
                    if (effect.parameters.ContainsKey("attackReduction"))
                    {
                        buff.attributeModifiers["attack"] = -Convert.ToSingle(effect.parameters["attackReduction"]);
                    }
                    if (effect.parameters.ContainsKey("defenseReduction"))
                    {
                        buff.attributeModifiers["defense"] = -Convert.ToSingle(effect.parameters["defenseReduction"]);
                    }
                    break;
            }
        }
    }

    #endregion
}
