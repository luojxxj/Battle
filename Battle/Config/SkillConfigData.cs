using System;
using System.Collections.Generic;
using Server.Battle.Skill;

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
        
        /// <summary>技能显示名称，用于UI显示和日志记录</summary>
        public string skillName;
        
        /// <summary>技能详细描述，说明技能效果和使用方法</summary>
        public string description;
        
        /// <summary>技能类型，决定技能的触发机制</summary>
        public SkillType skillType;
        
        /// <summary>技能消耗配置，包括消耗类型和数值</summary>
        public CostData cost;
        
        /// <summary>技能冷却回合数，0表示无冷却，建议范围0-10</summary>
        public int cooldown;
        
        /// <summary>AI使用优先级，数值越高优先级越高，范围1-10</summary>
        public int priority;
        
        /// <summary>目标选择类型，决定技能作用的目标范围</summary>
        public TargetType targetType;
        
        /// <summary>最大作用目标数量，主要用于AOE技能限制</summary>
        public int maxTargets;
        
        /// <summary>技能释放条件列表，所有条件都满足才能释放</summary>
        public List<SkillConditionData> conditions;
        
        /// <summary>技能效果列表，一个技能可以包含多种效果</summary>
        public List<SkillEffectData> effects;
        
        /// <summary>AI使用此技能的策略配置</summary>
        public AISkillConfig aiConfig;
    }

    /// <summary>
    /// 技能消耗配置
    /// 定义技能释放需要消耗的资源类型和数量
    /// </summary>
    [Serializable]
    public class CostData
    {
        /// <summary>消耗资源类型（法力、生命、能量等）</summary>
        public CostType type;
        
        /// <summary>消耗数值，必须大于等于0</summary>
        public int value;
    }

    /// <summary>
    /// 技能效果配置
    /// 定义技能可以产生的各种效果，包括伤害、治疗、Buff等
    /// 一个技能可以包含多个效果，每个效果都有独立的触发概率
    /// </summary>
    [Serializable]
    public class SkillEffectData
    {
        /// <summary>效果类型（伤害、治疗、Buff、Debuff等）</summary>
        public EffectType effectType;
        
        /// <summary>伤害类型（物理、魔法、真实等）</summary>
        public DamageType damageType;
        
        /// <summary>效果触发概率，取值范围0-1，1表示100%触发</summary>
        public float probability;
        
        /// <summary>特殊效果标识符，用于自定义效果类型</summary>
        public string effectId;
        
        /// <summary>效果基础数值（伤害值、治疗量等）</summary>
        public int baseValue;
        
        /// <summary>数值缩放依赖的属性类型（攻击力、法术攻击力等）</summary>
        public AttributeType scaleType;
        
        /// <summary>属性缩放系数，最终数值=基础数值+属性值×缩放系数</summary>
        public float scaleValue;
        
        /// <summary>是否可以触发暴击</summary>
        public bool canCrit;
        
        /// <summary>效果元素类型，影响伤害计算和抗性</summary>
        public ElementType element;
        
        /// <summary>是否无视目标防御力</summary>
        public bool ignoreDefense;
        
        /// <summary>是否无视目标元素抗性</summary>
        public bool ignoreResistance;
        
        /// <summary>关联的Buff/Debuff标识符</summary>
        public string buffId;
        
        /// <summary>效果持续回合数，0表示立即生效</summary>
        public int duration;
        
        /// <summary>同类型效果的叠加规则</summary>
        public StackType stackType;
        
        /// <summary>最大叠加层数</summary>
        public int maxStacks;
        
        /// <summary>每回合造成的伤害或治疗（DOT/HOT效果）</summary>
        public int tickDamage;
        
        /// <summary>效果范围（AOE技能的作用范围）</summary>
        public int effectRange;
        
        /// <summary>延迟生效时间（单位：回合）</summary>
        public int delayRounds;
        
        /// <summary>属性修正列表，用于Buff/Debuff改变单位属性</summary>
        public AttributeModifierData[] attributeModifiers;
        
        /// <summary>特殊效果配置，用于复杂的自定义效果</summary>
        public SpecialEffectData specialEffect;
        
        /// <summary>效果音效ID</summary>
        public string soundEffectId;
        
        /// <summary>效果特效ID</summary>
        public string visualEffectId;
    }

    /// <summary>
    /// 战斗单位基础配置
    /// 定义战斗单位的基础属性和职业信息
    /// </summary>
    [Serializable]
    public class BattleUnitConfig
    {
        /// <summary>单位唯一ID</summary>
        public int unitId;
        
        /// <summary>单位名称</summary>
        public string unitName;
        
        /// <summary>单位职业类型</summary>
        public UnitClass unitClass;
        
        /// <summary>战斗位置（前排/后排）</summary>
        public BattlePosition position;
        
        /// <summary>等级</summary>
        public int level;
        
        /// <summary>基础属性配置</summary>
        public UnitBaseAttributes baseAttributes;
        
        /// <summary>可使用的技能ID列表</summary>
        public List<int> skillIds;
        
        /// <summary>元素抗性配置</summary>
        public ElementResistanceData elementResistance;
        
        /// <summary>AI行为配置</summary>
        public UnitAIConfig aiConfig;
    }

    /// <summary>
    /// 单位基础属性配置
    /// 定义战斗单位的所有基础数值属性
    /// </summary>
    [Serializable]
    public class UnitBaseAttributes
    {
        /// <summary>最大生命值</summary>
        public int maxHp;
        
        /// <summary>最大法力值</summary>
        public int maxMp;
        
        /// <summary>物理攻击力</summary>
        public int attack;
        
        /// <summary>法术攻击力</summary>
        public int magicAttack;
        
        /// <summary>物理防御力</summary>
        public int defense;
        
        /// <summary>法术防御力</summary>
        public int magicDefense;
        
        /// <summary>速度</summary>
        public int speed;
        
        /// <summary>暴击率（0-1）</summary>
        public float critRate;
        
        /// <summary>暴击伤害倍率</summary>
        public float critDamage;
        
        /// <summary>命中率（0-1）</summary>
        public float hitRate;
        
        /// <summary>闪避率（0-1）</summary>
        public float dodgeRate;
    }

    /// <summary>
    /// 元素抗性配置
    /// 定义单位对各种元素伤害的抗性
    /// </summary>
    [Serializable]
    public class ElementResistanceData
    {
        /// <summary>火元素抗性（0-1，0表示无抗性，1表示完全免疫）</summary>
        public float fireResistance;
        
        /// <summary>水元素抗性</summary>
        public float waterResistance;
        
        /// <summary>土元素抗性</summary>
        public float earthResistance;
        
        /// <summary>风元素抗性</summary>
        public float airResistance;
        
        /// <summary>光元素抗性</summary>
        public float lightResistance;
        
        /// <summary>暗元素抗性</summary>
        public float darkResistance;
        
        /// <summary>物理抗性</summary>
        public float physicalResistance;
    }

    /// <summary>
    /// 单位AI配置
    /// 定义AI单位的行为模式和偏好
    /// </summary>
    [Serializable]
    public class UnitAIConfig
    {
        /// <summary>AI类型（进攻型、防守型、辅助型等）</summary>
        public AIType aiType;
        
        /// <summary>攻击偏好权重</summary>
        public float aggressionWeight;
        
        /// <summary>防御偏好权重</summary>
        public float defenseWeight;
        
        /// <summary>支援偏好权重</summary>
        public float supportWeight;
        
        /// <summary>技能使用策略</summary>
        public SkillUsageStrategy skillStrategy;
        
        /// <summary>目标选择偏好</summary>
        public TargetPreference targetPreference;
    }

    /// <summary>
    /// 技能释放条件配置
    /// 定义技能能够释放的前置条件，支持多种条件类型和比较操作
    /// 所有条件都必须满足技能才能释放
    /// </summary>
    [Serializable]
    public class SkillConditionData
    {
        /// <summary>条件检查类型（血量、法力、单位数量等）</summary>
        public ConditionType conditionType;
        
        /// <summary>数值比较方式（大于、小于、等于等）</summary>
        public ComparisonType comparison;
        
        /// <summary>比较的目标数值</summary>
        public float value;
        
        /// <summary>目标标签，用于特定条件的额外参数</summary>
        public string targetTag;
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

    /// <summary>
    /// 特殊效果配置
    /// 用于实现复杂的自定义效果，如吸血、连锁、反伤等
    /// 通过参数字典传递效果所需的各种参数
    /// </summary>
    [Serializable]
    public class SpecialEffectData
    {
        /// <summary>特殊效果类型</summary>
        public SpecialEffectType effectType;
        
        /// <summary>整数参数字典，用于传递整数类型参数</summary>
        public Dictionary<string, int> intParameters;
        
        /// <summary>浮点数参数字典，用于传递浮点数类型参数</summary>
        public Dictionary<string, float> floatParameters;
        
        /// <summary>字符串参数字典，用于传递字符串类型参数</summary>
        public Dictionary<string, string> stringParameters;
        
        /// <summary>布尔参数字典，用于传递布尔类型参数</summary>
        public Dictionary<string, bool> boolParameters;

        public SpecialEffectData()
        {
            intParameters = new Dictionary<string, int>();
            floatParameters = new Dictionary<string, float>();
            stringParameters = new Dictionary<string, string>();
            boolParameters = new Dictionary<string, bool>();
        }

        /// <summary>
        /// 获取整数参数
        /// </summary>
        public int GetIntParameter(string key, int defaultValue = 0)
        {
            return intParameters.ContainsKey(key) ? intParameters[key] : defaultValue;
        }

        /// <summary>
        /// 获取浮点数参数
        /// </summary>
        public float GetFloatParameter(string key, float defaultValue = 0f)
        {
            return floatParameters.ContainsKey(key) ? floatParameters[key] : defaultValue;
        }

        /// <summary>
        /// 获取字符串参数
        /// </summary>
        public string GetStringParameter(string key, string defaultValue = "")
        {
            return stringParameters.ContainsKey(key) ? stringParameters[key] : defaultValue;
        }

        /// <summary>
        /// 获取布尔参数
        /// </summary>
        public bool GetBoolParameter(string key, bool defaultValue = false)
        {
            return boolParameters.ContainsKey(key) ? boolParameters[key] : defaultValue;
        }
    }

    /// <summary>
    /// AI技能使用配置
    /// 定义AI在什么情况下会使用此技能，以及使用的偏好设置
    /// 用于实现智能的AI战斗决策
    /// </summary>
    [Serializable]
    public class AISkillConfig
    {
        /// <summary>自身血量阈值，血量低于此值时更倾向于使用此技能</summary>
        public float selfHpThreshold;
        
        /// <summary>敌人血量阈值，敌人血量在此值以下时更倾向于使用</summary>
        public float enemyHpThreshold;
        
        /// <summary>最少敌人数量要求，敌人数量达到此值才考虑使用</summary>
        public int minEnemyCount;
        
        /// <summary>最少队友数量要求，队友数量达到此值才考虑使用</summary>
        public int minAllyCount;
        
        /// <summary>基础使用概率，取值0-1</summary>
        public float useProbability;
        
        /// <summary>优先级修正值，影响技能在AI决策中的权重</summary>
        public float priorityModifier;
        
        /// <summary>目标选择偏好，决定AI优先攻击什么样的目标</summary>
        public TargetPreference targetPreference;
    }

    #region 枚举定义

    /// <summary>
    /// 职业类型枚举
    /// 定义战斗单位的职业分类，对应游戏中的五大职业
    /// </summary>
    public enum UnitClass
    {
        /// <summary>济世丞 - 战士职业，高生命值和防御力</summary>
        Warrior = 1,
        
        /// <summary>穿云卫 - 法师职业，高法术攻击力</summary>
        Mage = 2,
        
        /// <summary>铁壁营 - 坦克职业，超高防御和生命值</summary>
        Tank = 3,
        
        /// <summary>韬略使 - 辅助职业，提供支援和治疗</summary>
        Support = 4,
        
        /// <summary>影刃侍 - 刺客职业，高速度和暴击</summary>
        Assassin = 5
    }

    /// <summary>
    /// 战斗位置枚举
    /// 定义战斗单位在战场上的位置，影响技能目标选择
    /// </summary>
    public enum BattlePosition
    {
        /// <summary>前排位置 - 通常是坦克和战士</summary>
        FrontLine = 1,
        
        /// <summary>后排位置 - 通常是法师、射手、辅助</summary>
        BackLine = 2
    }

    /// <summary>
    /// 战斗阶段枚举
    /// 定义战斗的不同阶段，用于技能触发和状态管理
    /// </summary>
    public enum BattlePhase
    {
        /// <summary>战斗准备阶段</summary>
        Preparing = 0,
        
        /// <summary>战斗开始阶段</summary>
        BattleStart = 1,
        
        /// <summary>回合开始阶段</summary>
        RoundStart = 2,
        
        /// <summary>单位行动阶段</summary>
        UnitAction = 3,
        
        /// <summary>回合结束阶段</summary>
        RoundEnd = 4,
        
        /// <summary>战斗结束阶段</summary>
        BattleEnd = 5
    }

    /// <summary>
    /// 消耗类型枚举
    /// 定义技能释放需要消耗的资源类型
    /// </summary>
    public enum CostType
    {
        /// <summary>无消耗</summary>
        None,
        
        /// <summary>法力值消耗</summary>
        Mp,
        
        /// <summary>生命值消耗</summary>
        Hp,
        
        /// <summary>能量消耗</summary>
        Energy,
        
        /// <summary>连击点消耗</summary>
        Combo
    }

    /// <summary>
    /// 目标类型枚举
    /// 定义技能可以作用的目标范围和选择方式
    /// 用于技能系统的目标选择逻辑
    /// 数值对应策划配置的目标类型ID
    /// </summary>
    public enum TargetType
    {
        /// <summary>单个敌人 - 选择一个敌方单位</summary>
        SingleEnemy = 0,
        
        /// <summary>我方全体 - 作用于所有友方单位</summary>
        AllAllies = 1,
        
        /// <summary>敌方全体 - 作用于所有敌方单位</summary>
        AllEnemies = 2,
        
        /// <summary>我方随机 - 随机选择友方单位</summary>
        RandomAllies = 3,
        
        /// <summary>我方前排 - 选择前排位置的友方单位</summary>
        AllyFrontLine = 4,
        
        /// <summary>我方后排 - 选择后排位置的友方单位</summary>
        AllyBackLine = 5,
        
        /// <summary>敌方前排 - 选择前排位置的敌方单位</summary>
        EnemyFrontLine = 6,
        
        /// <summary>敌方后排 - 选择后排位置的敌方单位</summary>
        EnemyBackLine = 7,
        
        /// <summary>敌方随机 - 随机选择敌方单位</summary>
        RandomEnemies = 8,
        
        /// <summary>济世丞 - 选择战士职业单位</summary>
        Warrior = 9,
        
        /// <summary>穿云卫 - 选择法师职业单位</summary>
        Mage = 10,
        
        /// <summary>铁壁营 - 选择坦克职业单位</summary>
        Tank = 11,
        
        /// <summary>韬略使 - 选择辅助职业单位</summary>
        Support = 12,
        
        /// <summary>影刃侍 - 选择刺客职业单位</summary>
        Assassin = 13,
        
        /// <summary>自己 - 技能作用于释放者自身</summary>
        Self = 14,
        
        /// <summary>我方当前血量最低 - 选择友方血量最少的单位</summary>
        AllyLowestHp = 15,
        
        /// <summary>敌方攻击力最高 - 选择敌方攻击力最高的单位</summary>
        EnemyHighestAttack = 16,
        
        /// <summary>敌方当前生命最高 - 选择敌方当前血量最多的单位</summary>
        EnemyHighestCurrentHp = 17,
        
        /// <summary>单个队友</summary>
        SingleAlly = 18,
        
        /// <summary>敌方当前生命最低 - 选择敌方当前血量最少的单位</summary>
        EnemyLowestCurrentHp = 19,
        
        /// <summary>我方攻击力最高 - 选择友方攻击力最高的单位</summary>
        AllyHighestAttack = 20,
    }

    /// <summary>
    /// 属性类型枚举
    /// 定义战斗单位的各种属性，用于技能效果和Buff修正
    /// </summary>
    public enum AttributeType
    {
        /// <summary>
        /// 生命值
        /// </summary>
        Hp = 1001,

        /// <summary>
        /// 攻击
        /// </summary>
        Attack = 1002,

        /// <summary>
        /// 防御
        /// </summary>
        Defense = 1003,

        /// <summary>
        /// 速度
        /// </summary>
        Speed = 1004,

        /// <summary>
        /// 怒气
        /// </summary>
        Mp = 1005,

        /// <summary>
        /// 生命百分比
        /// </summary>
        HpRate = 2001,

        /// <summary>
        /// 攻击百分比
        /// </summary>
        AttackRate = 2002,

        /// <summary>
        /// 防御百分比
        /// </summary>
        DefenseRate = 2003,

        /// <summary>
        /// 速度百分比
        /// </summary>
        SpeedRate = 2004,

        /// <summary>
        /// 怒气百分比
        /// </summary>
        MpRate = 2005,

        /// <summary>
        /// 命中率
        /// </summary>
        HitRate = 3001,

        /// <summary>
        /// 闪避率
        /// </summary>
        DodgeRate = 3002,

        /// <summary>
        /// 反击率
        /// </summary>
        RetaliateRate = 3003,

        /// <summary>
        /// 反击抵抗率
        /// </summary>
        ResistRetaliateRate = 3004,

        /// <summary>
        /// 连击率
        /// </summary>
        ComboRate = 3005,

        /// <summary>
        /// 连击抵抗率
        /// </summary>
        ResistComboRate = 3006,

        /// <summary>
        /// 伤害增加
        /// </summary>
        FinalDamageRate = 3007,

        /// <summary>
        /// 伤害减免
        /// </summary>
        FinalReduceRate = 3008,

        /// <summary>
        /// 技能增伤
        /// </summary>
        FinalSkillDamageRate = 3009,

        /// <summary>
        /// 技能免伤
        /// </summary>
        FinalSkillReduceRate = 3010,

        /// <summary>
        /// 暴击率
        /// </summary>
        CriticalRate = 3011,

        /// <summary>
        /// 暴击抵抗率
        /// </summary>
        ResistCriticalRate = 3012,

        /// <summary>
        /// 暴击伤害
        /// </summary>
        CriticalDamageRate = 3013,

        /// <summary>
        /// 暴击伤害抵抗
        /// </summary>
        ResistCriticalDamageRate = 3014,

        /// <summary>
        /// 控制增强
        /// </summary>
        ControlCountEnhance = 1007,

        /// <summary>
        /// 控制降低
        /// </summary>
        ControlCountReduce = 1008,

        /// <summary>
        /// vip增伤
        /// </summary>
        VipDamageRate = 3015,

        /// <summary>
        /// vip减伤
        /// </summary>
        VipReduceRate = 3016,

        /// <summary>
        /// 受疗加成
        /// </summary>
        HealedRate = 3017,

        /// <summary>
        /// 治疗加成
        /// </summary>
        HealRate = 3018,

        /// <summary>
        /// 吸血倍率
        /// </summary>
        BloodSuckRate = 3019,

        /// <summary>
        /// 格挡率
        /// </summary>
        BlockRate = 3020,

        /// <summary>
        /// 贯穿率
        /// </summary>
        PierceRate = 3021
    }

    /// <summary>
    /// 元素类型枚举
    /// 定义技能和伤害的元素属性，用于元素相克计算
    /// </summary>
    public enum ElementType
    {
        /// <summary>物理 - 物理攻击，无元素属性</summary>
        Physical,
        
        /// <summary>火 - 火元素攻击</summary>
        Fire,
        
        /// <summary>水 - 水元素攻击</summary>
        Water,
        
        /// <summary>土 - 土元素攻击</summary>
        Earth,
        
        /// <summary>风 - 风元素攻击</summary>
        Air,
        
        /// <summary>光 - 光元素攻击</summary>
        Light,
        
        /// <summary>暗 - 暗元素攻击</summary>
        Dark,
        
        /// <summary>无属性 - 中性伤害，不受元素影响</summary>
        Neutral
    }

    /// <summary>
    /// 叠加类型枚举
    /// 定义相同效果之间的叠加规则
    /// </summary>
    public enum StackType
    {
        /// <summary>不叠加 - 新效果覆盖旧效果</summary>
        None,
        
        /// <summary>叠加 - 层数和持续时间都叠加</summary>
        Stack,
        
        /// <summary>刷新 - 刷新持续时间，但不叠加层数</summary>
        Refresh,
        
        /// <summary>独立存在 - 可以多个同时存在，互不影响</summary>
        Independent
    }

    /// <summary>
    /// 条件类型枚举
    /// 定义技能释放条件的检查类型
    /// </summary>
    public enum ConditionType
    {
        /// <summary>自身血量条件</summary>
        SelfHp,
        
        /// <summary>自身法力条件</summary>
        SelfMp,
        
        /// <summary>敌人数量条件</summary>
        EnemyCount,
        
        /// <summary>队友数量条件</summary>
        AllyCount,
        
        /// <summary>是否拥有指定Buff</summary>
        HasBuff,
        
        /// <summary>是否拥有指定Debuff</summary>
        HasDebuff,
        
        /// <summary>当前回合数条件</summary>
        RoundNumber,
        
        /// <summary>敌人血量条件</summary>
        EnemyHp,
        
        /// <summary>随机概率条件</summary>
        Random
    }

    /// <summary>
    /// 比较类型枚举
    /// 定义数值比较的方式
    /// </summary>
    public enum ComparisonType
    {
        /// <summary>等于</summary>
        Equal,
        
        /// <summary>不等于</summary>
        NotEqual,
        
        /// <summary>大于</summary>
        Greater,
        
        /// <summary>大于等于</summary>
        GreaterEqual,
        
        /// <summary>小于</summary>
        Less,
        
        /// <summary>小于等于</summary>
        LessEqual
    }

    /// <summary>
    /// 修正类型枚举
    /// 定义属性修正的计算方式
    /// </summary>
    public enum ModifierType
    {
        /// <summary>加法修正 - 在原值基础上增加</summary>
        Add,
        
        /// <summary>乘法修正 - 对原值进行百分比修正</summary>
        Multiply,
        
        /// <summary>设置值 - 直接设置为指定数值</summary>
        Set
    }

    /// <summary>
    /// 特殊效果类型枚举
    /// 定义复杂的自定义技能效果
    /// </summary>
    public enum SpecialEffectType
    {
        /// <summary>吸血效果 - 造成伤害的同时恢复生命</summary>
        Vampiric,
        
        /// <summary>连锁效果 - 攻击会跳跃到其他目标</summary>
        Chain,
        
        /// <summary>反伤效果 - 将受到的伤害反弹给攻击者</summary>
        Reflect,
        
        /// <summary>复活效果 - 使死亡单位复活</summary>
        Revive,
        
        /// <summary>传送效果 - 改变单位位置</summary>
        Teleport,
        
        /// <summary>变身效果 - 改变单位形态或技能</summary>
        Transform,
        
        /// <summary>复制效果 - 复制其他单位的技能或属性</summary>
        Copy,
        
        /// <summary>偷取效果 - 窃取目标的Buff或属性</summary>
        Steal,
        
        /// <summary>交换效果 - 与目标交换血量或位置</summary>
        Exchange,
        
        /// <summary>献祭效果 - 牺牲自己强化队友</summary>
        Sacrifice
    }

    /// <summary>
    /// 目标偏好枚举
    /// 定义AI选择目标时的偏好策略
    /// </summary>
    public enum TargetPreference
    {
        /// <summary>随机选择</summary>
        Random,
        
        /// <summary>优先选择血量最低的目标</summary>
        LowestHp,
        
        /// <summary>优先选择血量最高的目标</summary>
        HighestHp,
        
        /// <summary>优先选择法力最低的目标</summary>
        LowestMp,
        
        /// <summary>优先选择法力最高的目标</summary>
        HighestMp,
        
        /// <summary>优先选择威胁最高的目标</summary>
        HighestThreat,
        
        /// <summary>优先选择最弱的目标</summary>
        Weakest,
        
        /// <summary>优先选择最强的目标</summary>
        Strongest
    }

    /// <summary>
    /// AI类型枚举
    /// 定义AI的基本行为模式
    /// </summary>
    public enum AIType
    {
        /// <summary>进攻型AI - 优先攻击敌人</summary>
        Aggressive,
        
        /// <summary>防守型AI - 优先保护队友</summary>
        Defensive,
        
        /// <summary>辅助型AI - 优先使用辅助技能</summary>
        Support,
        
        /// <summary>平衡型AI - 根据情况灵活应对</summary>
        Balanced,
        
        /// <summary>控制型AI - 优先使用控制技能</summary>
        Control
    }

    /// <summary>
    /// 技能使用策略枚举
    /// 定义AI使用技能的基本策略
    /// </summary>
    public enum SkillUsageStrategy
    {
        /// <summary>优先使用伤害技能</summary>
        DamageFirst,
        
        /// <summary>优先使用治疗技能</summary>
        HealFirst,
        
        /// <summary>优先使用Buff技能</summary>
        BuffFirst,
        
        /// <summary>优先使用Debuff技能</summary>
        DebuffFirst,
        
        /// <summary>节约技能，只在关键时刻使用</summary>
        Conservative,
        
        /// <summary>积极使用技能</summary>
        Aggressive,
        
        /// <summary>智能选择，根据战况决定</summary>
        Smart
    }

    /// <summary>
    /// 伤害类型枚举
    /// 定义伤害的具体分类，用于更精确的伤害计算
    /// </summary>
    public enum DamageType
    {
        /// <summary>物理伤害</summary>
        Physical,
        
        /// <summary>魔法伤害</summary>
        Magical,
        
        /// <summary>真实伤害（无视防御）</summary>
        True,
        
        /// <summary>持续伤害（DOT）</summary>
        OverTime,
        
        /// <summary>治疗</summary>
        Heal,
        
        /// <summary>护盾</summary>
        Shield
    }

    #endregion
}
