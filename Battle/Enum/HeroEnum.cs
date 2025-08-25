
namespace Battle.Enum
{
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
}
