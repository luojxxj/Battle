using Battle.Enum;

namespace Server.Battle.Config
{
    /// <summary>
    /// 技能配置数据结构 - 策划配置用
    /// 所有数值和效果都通过JSON配置，服务器只负责执行
    /// 支持回合制战斗系统，6v6最多12个单位的技能计算
    /// </summary>
    [Serializable]
    public class SkillConfigData
    {
        /// <summary>技能唯一标识符，必须大于0且全局唯一</summary>
        public int skillId;

        /// <summary>技能类型，决定技能的触发机制</summary>
        public SkillType skillType;

        /// <summary>技能释放条件</summary>
        public RevolutionType revolutionType;
        
        /// <summary>释放条件值</summary>
        public int revolutionCost;
        
        /// <summary>目标选择类型，决定技能作用的目标范围</summary>
        public TargetType targetType;
        
        /// <summary>范围</summary>
        public int targetCount;
        
        /// <summary>技能效果列表，一个技能可以包含多种效果</summary>
        public List<EffectData> effects;

        /// <summary>优先级</summary>
        public int priority;

        /// <summary>是否必定暴击</summary>
        public bool critical;

        /// <summary>是否必定命中</summary>
        public bool hit;
    }

    /// <summary>
    /// Buff配置数据
    /// </summary>
    [Serializable]
    public class BuffConfigData
    {
        public int buffId;
        public EffectType buffType;
        public bool canRemove;
        public int conRound;
        public bool canStack;
        public int stackLimit;
        public List<EffectData> effectList;
        public List<EffectData> endList;
    }

    /// <summary>
    /// 效果配置
    /// 定义技能可以产生的各种效果，包括伤害、治疗、Buff等
    /// 一个技能可以包含多个效果，每个效果都有独立的触发概率
    /// </summary>
    [Serializable]
    public class EffectData
    {
        /// <summary>效果类型（伤害、治疗、Buff、Debuff等）</summary>
        public EffectType effectType;

        /// <summary>效果参数</summary>
        public List<int> param;
    }

    /// <summary>
    /// 触发器配置
    /// </summary>
    [Serializable]
    public class TriggerConfigData
    {
        public int triggerId;
        public List<ConditionData> triggerList;
        public List<EffectData> effectList;
        public int triggerWeight;
    }

    /// <summary>
    /// 条件配置
    /// </summary>
    public class ConditionData
    {
        public ConditionType conditionType;
        public List<int> param;
    }

    /// <summary>
    /// 属性修正配置
    /// 用于Buff/Debuff效果修改单位属性
    /// 支持加法、乘法和直接设置三种修正方式
    /// </summary>
    [Serializable]
    public class AttributeModifierData
    {
        /// <summary>要修正的属性类型</summary>
        public AttributeType attributeType;
        
        /// <summary>修正方式（加法、乘法、设置值）</summary>
        public ModifierType modifierType;
        
        /// <summary>修正数值</summary>
        public float value;
        
        /// <summary>是否为百分比修正</summary>
        public bool isPercent;
    }
}
