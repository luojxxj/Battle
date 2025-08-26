
namespace Battle.Enum
{
    /// <summary>
    /// 技能类型
    /// </summary>
    public enum SkillType
    {
        Attack = 1,     // 普通攻击
        Active = 2,     // 主动技能
        Passive = 3,    // 被动技能
        Resident = 4       // 常驻技能
    }

    /// <summary>
    /// 效果类型
    /// </summary>
    public enum EffectType
    {
        Damage = 1,           // 伤害
        Heal = 2,            // 治疗
        AddBuff = 3,            // 增益
        Odds = 4,          // 概率触发
        Trigger = 5,          // 条件触发
        ChangeAttrPercentage = 6,          //百分比改变属性
        ChangeAtteFixed = 7,           //固定改变属性
        RemoveBuff = 8,       // 移除buff
        Revive = 9,          // 复活
        SkillRepeat = 10,      // 重复释放技能
        DisControl = 11,      // 免疫控制
        Control = 12,       // 控制效果
        Shield = 13,         // 护盾
        Taunt = 14,         // 嘲讽
        MinHpHeal = 15,      // 最小血量治疗
        RecordHarm = 16,      // 记录伤害
        Dispel = 17,           // 驱散
        Debuff = 18,           // 减益
    }

    /// <summary>
    /// 技能释放条件
    /// </summary>
    public enum RevolutionType
    {
        Mp = 0,          // 怒气值
        Cooldown = 1,    // 冷却时间
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
        
        /// <summary>敌方全体</summary>
        EnemyAll = 21,

        /// <summary>敌方随机</summary>
        EnemyRandom = 22,

        /// <summary>敌方血量最低</summary>
        EnemyLowestHp = 23,

        /// <summary>友方全体</summary>
        AllyAll = 24,

        /// <summary>友方随机</summary>
        AllyRandom = 25,
    }
}
