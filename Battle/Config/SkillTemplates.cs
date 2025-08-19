using System;
using System.Collections.Generic;
using Server.Battle.Skill;

namespace Server.Battle.Config
{
    /// <summary>
    /// 技能模板系统
    /// 提供预定义的技能模板，方便策划快速创建常见技能
    /// </summary>
    public static class SkillTemplates
    {
        /// <summary>
        /// 创建基础攻击技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="skillName">技能名称</param>
        /// <param name="attackScale">攻击力缩放系数</param>
        /// <returns>基础攻击技能配置</returns>
        public static SkillConfigData CreateBasicAttack(int skillId, string skillName, float attackScale = 1.0f)
        {
            return new SkillConfigData
            {
                skillId = skillId,
                skillName = skillName,
                description = "基础攻击技能",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.None, value = 0 },
                cooldown = 0,
                priority = 1,
                targetType = TargetType.SingleEnemy,
                maxTargets = 1,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Damage,
                        damageType = DamageType.Physical,
                        probability = 1.0f,
                        baseValue = 0,
                        scaleType = AttributeType.Attack,
                        scaleValue = attackScale,
                        canCrit = true,
                        element = ElementType.Physical,
                        ignoreDefense = false,
                        ignoreResistance = false,
                        duration = 0,
                        effectRange = 1,
                        delayRounds = 0
                    }
                },
                aiConfig = CreateDefaultAIConfig()
            };
        }
        
        /// <summary>
        /// 创建治疗技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="skillName">技能名称</param>
        /// <param name="healValue">治疗基础值</param>
        /// <param name="mpCost">法力消耗</param>
        /// <param name="cooldown">冷却时间</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>治疗技能配置</returns>
        public static SkillConfigData CreateHealSkill(int skillId, string skillName, int healValue, int mpCost, int cooldown, TargetType targetType = TargetType.AllyLowestHp)
        {
            return new SkillConfigData
            {
                skillId = skillId,
                skillName = skillName,
                description = "治疗技能",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = mpCost },
                cooldown = cooldown,
                priority = 8,
                targetType = targetType,
                maxTargets = targetType == TargetType.AllAllies ? 6 : 1,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Heal,
                        damageType = DamageType.Heal,
                        probability = 1.0f,
                        baseValue = healValue,
                        scaleType = AttributeType.Attack,
                        scaleValue = 0.5f,
                        canCrit = false,
                        element = ElementType.Light,
                        ignoreDefense = true,
                        ignoreResistance = true,
                        duration = 0,
                        effectRange = targetType == TargetType.AllAllies ? 6 : 1,
                        delayRounds = 0
                    }
                },
                aiConfig = CreateHealerAIConfig()
            };
        }
        
        /// <summary>
        /// 创建AOE攻击技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="skillName">技能名称</param>
        /// <param name="baseDamage">基础伤害</param>
        /// <param name="attackScale">攻击力缩放</param>
        /// <param name="mpCost">法力消耗</param>
        /// <param name="cooldown">冷却时间</param>
        /// <param name="element">元素类型</param>
        /// <returns>AOE攻击技能配置</returns>
        public static SkillConfigData CreateAOEAttack(int skillId, string skillName, int baseDamage, float attackScale, int mpCost, int cooldown, ElementType element = ElementType.Physical)
        {
            return new SkillConfigData
            {
                skillId = skillId,
                skillName = skillName,
                description = "群体攻击技能",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = mpCost },
                cooldown = cooldown,
                priority = 6,
                targetType = TargetType.AllEnemies,
                maxTargets = 6,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Damage,
                        damageType = element == ElementType.Physical ? DamageType.Physical : DamageType.Magical,
                        probability = 1.0f,
                        baseValue = baseDamage,
                        scaleType = element == ElementType.Physical ? AttributeType.Attack : AttributeType.Attack,
                        scaleValue = attackScale,
                        canCrit = true,
                        element = element,
                        ignoreDefense = false,
                        ignoreResistance = false,
                        duration = 0,
                        effectRange = 6,
                        delayRounds = 0
                    }
                },
                aiConfig = CreateAggresiveAIConfig()
            };
        }
        
        /// <summary>
        /// 创建Buff技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="skillName">技能名称</param>
        /// <param name="buffId">Buff标识符</param>
        /// <param name="duration">持续时间</param>
        /// <param name="mpCost">法力消耗</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="attributeModifiers">属性修正</param>
        /// <returns>Buff技能配置</returns>
        public static SkillConfigData CreateBuffSkill(int skillId, string skillName, string buffId, int duration, int mpCost, TargetType targetType, AttributeModifierData[] attributeModifiers)
        {
            return new SkillConfigData
            {
                skillId = skillId,
                skillName = skillName,
                description = "增益技能",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = mpCost },
                cooldown = duration + 1,
                priority = 7,
                targetType = targetType,
                maxTargets = targetType == TargetType.AllAllies ? 6 : 1,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Buff,
                        probability = 1.0f,
                        buffId = buffId,
                        duration = duration,
                        stackType = StackType.Refresh,
                        maxStacks = 1,
                        attributeModifiers = attributeModifiers,
                        effectRange = targetType == TargetType.AllAllies ? 6 : 1,
                        delayRounds = 0
                    }
                },
                aiConfig = CreateSupportAIConfig()
            };
        }
        
        /// <summary>
        /// 创建Debuff技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="skillName">技能名称</param>
        /// <param name="debuffId">Debuff标识符</param>
        /// <param name="duration">持续时间</param>
        /// <param name="probability">触发概率</param>
        /// <param name="mpCost">法力消耗</param>
        /// <param name="attributeModifiers">属性修正</param>
        /// <returns>Debuff技能配置</returns>
        public static SkillConfigData CreateDebuffSkill(int skillId, string skillName, string debuffId, int duration, float probability, int mpCost, AttributeModifierData[] attributeModifiers)
        {
            return new SkillConfigData
            {
                skillId = skillId,
                skillName = skillName,
                description = "减益技能",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = mpCost },
                cooldown = 3,
                priority = 5,
                targetType = TargetType.SingleEnemy,
                maxTargets = 1,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Debuff,
                        probability = probability,
                        buffId = debuffId,
                        duration = duration,
                        stackType = StackType.Stack,
                        maxStacks = 3,
                        attributeModifiers = attributeModifiers,
                        effectRange = 1,
                        delayRounds = 0
                    }
                },
                aiConfig = CreateControlAIConfig()
            };
        }
        
        /// <summary>
        /// 创建DOT技能（持续伤害）
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="skillName">技能名称</param>
        /// <param name="initialDamage">初始伤害</param>
        /// <param name="tickDamage">每回合伤害</param>
        /// <param name="duration">持续时间</param>
        /// <param name="element">元素类型</param>
        /// <param name="mpCost">法力消耗</param>
        /// <returns>DOT技能配置</returns>
        public static SkillConfigData CreateDOTSkill(int skillId, string skillName, int initialDamage, int tickDamage, int duration, ElementType element, int mpCost)
        {
            return new SkillConfigData
            {
                skillId = skillId,
                skillName = skillName,
                description = "持续伤害技能",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = mpCost },
                cooldown = 2,
                priority = 4,
                targetType = TargetType.SingleEnemy,
                maxTargets = 1,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    // 初始伤害
                    new SkillEffectData
                    {
                        effectType = EffectType.Damage,
                        damageType = DamageType.Magical,
                        probability = 1.0f,
                        baseValue = initialDamage,
                        scaleType = AttributeType.Attack,
                        scaleValue = 0.8f,
                        canCrit = true,
                        element = element,
                        duration = 0,
                        effectRange = 1,
                        delayRounds = 0
                    },
                    // DOT效果
                    new SkillEffectData
                    {
                        effectType = EffectType.Debuff,
                        probability = 1.0f,
                        buffId = $"dot_{element.ToString().ToLower()}",
                        duration = duration,
                        stackType = StackType.Refresh,
                        maxStacks = 1,
                        tickDamage = tickDamage,
                        element = element,
                        effectRange = 1,
                        delayRounds = 0
                    }
                },
                aiConfig = CreateDefaultAIConfig()
            };
        }
        
        /// <summary>
        /// 创建护盾技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="skillName">技能名称</param>
        /// <param name="shieldValue">护盾值</param>
        /// <param name="duration">持续时间</param>
        /// <param name="mpCost">法力消耗</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>护盾技能配置</returns>
        public static SkillConfigData CreateShieldSkill(int skillId, string skillName, int shieldValue, int duration, int mpCost, TargetType targetType = TargetType.Self)
        {
            return new SkillConfigData
            {
                skillId = skillId,
                skillName = skillName,
                description = "护盾技能",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = mpCost },
                cooldown = duration + 2,
                priority = 8,
                targetType = targetType,
                maxTargets = targetType == TargetType.AllAllies ? 6 : 1,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Shield,
                        damageType = DamageType.Shield,
                        probability = 1.0f,
                        baseValue = shieldValue,
                        scaleType = AttributeType.Attack,
                        scaleValue = 0.3f,
                        duration = duration,
                        stackType = StackType.None,
                        maxStacks = 1,
                        effectRange = targetType == TargetType.AllAllies ? 6 : 1,
                        delayRounds = 0
                    }
                },
                aiConfig = CreateDefensiveAIConfig()
            };
        }
        
        /// <summary>
        /// 创建连锁攻击技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="skillName">技能名称</param>
        /// <param name="baseDamage">基础伤害</param>
        /// <param name="chainCount">连锁次数</param>
        /// <param name="damageReduction">伤害衰减</param>
        /// <param name="mpCost">法力消耗</param>
        /// <returns>连锁攻击技能配置</returns>
        public static SkillConfigData CreateChainAttack(int skillId, string skillName, int baseDamage, int chainCount, float damageReduction, int mpCost)
        {
            var specialEffect = new SpecialEffectData
            {
                effectType = SpecialEffectType.Chain
            };
            specialEffect.intParameters["chainCount"] = chainCount;
            specialEffect.floatParameters["damageReduction"] = damageReduction;
            
            return new SkillConfigData
            {
                skillId = skillId,
                skillName = skillName,
                description = "连锁攻击技能",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = mpCost },
                cooldown = 3,
                priority = 5,
                targetType = TargetType.SingleEnemy,
                maxTargets = chainCount + 1,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Special,
                        damageType = DamageType.Magical,
                        probability = 1.0f,
                        baseValue = baseDamage,
                        scaleType = AttributeType.Attack,
                        scaleValue = 1.2f,
                        canCrit = true,
                        element = ElementType.Air,
                        specialEffect = specialEffect,
                        effectRange = chainCount + 1,
                        delayRounds = 0
                    }
                },
                aiConfig = CreateAggresiveAIConfig()
            };
        }
        
        #region AI配置模板
        
        /// <summary>
        /// 创建默认AI配置
        /// </summary>
        private static AISkillConfig CreateDefaultAIConfig()
        {
            return new AISkillConfig
            {
                selfHpThreshold = 1.0f,
                enemyHpThreshold = 1.0f,
                minEnemyCount = 1,
                minAllyCount = 0,
                useProbability = 0.8f,
                priorityModifier = 0.0f,
                targetPreference = TargetPreference.Random
            };
        }
        
        /// <summary>
        /// 创建治疗者AI配置
        /// </summary>
        private static AISkillConfig CreateHealerAIConfig()
        {
            return new AISkillConfig
            {
                selfHpThreshold = 0.7f,
                enemyHpThreshold = 1.0f,
                minEnemyCount = 0,
                minAllyCount = 1,
                useProbability = 0.9f,
                priorityModifier = 2.0f,
                targetPreference = TargetPreference.LowestHp
            };
        }
        
        /// <summary>
        /// 创建攻击型AI配置
        /// </summary>
        private static AISkillConfig CreateAggresiveAIConfig()
        {
            return new AISkillConfig
            {
                selfHpThreshold = 0.3f,
                enemyHpThreshold = 1.0f,
                minEnemyCount = 2,
                minAllyCount = 0,
                useProbability = 0.85f,
                priorityModifier = 1.0f,
                targetPreference = TargetPreference.LowestHp
            };
        }
        
        /// <summary>
        /// 创建支援型AI配置
        /// </summary>
        private static AISkillConfig CreateSupportAIConfig()
        {
            return new AISkillConfig
            {
                selfHpThreshold = 0.8f,
                enemyHpThreshold = 1.0f,
                minEnemyCount = 0,
                minAllyCount = 2,
                useProbability = 0.75f,
                priorityModifier = 1.5f,
                targetPreference = TargetPreference.Weakest
            };
        }
        
        /// <summary>
        /// 创建控制型AI配置
        /// </summary>
        private static AISkillConfig CreateControlAIConfig()
        {
            return new AISkillConfig
            {
                selfHpThreshold = 0.6f,
                enemyHpThreshold = 0.8f,
                minEnemyCount = 1,
                minAllyCount = 0,
                useProbability = 0.7f,
                priorityModifier = 0.5f,
                targetPreference = TargetPreference.Strongest
            };
        }
        
        /// <summary>
        /// 创建防御型AI配置
        /// </summary>
        private static AISkillConfig CreateDefensiveAIConfig()
        {
            return new AISkillConfig
            {
                selfHpThreshold = 0.5f,
                enemyHpThreshold = 1.0f,
                minEnemyCount = 0,
                minAllyCount = 1,
                useProbability = 0.9f,
                priorityModifier = 1.8f,
                targetPreference = TargetPreference.LowestHp
            };
        }
        
        #endregion
        
        #region 快速创建属性修正
        
        /// <summary>
        /// 创建攻击力修正
        /// </summary>
        public static AttributeModifierData CreateAttackModifier(float value, bool isPercent = false)
        {
            return new AttributeModifierData
            {
                attributeType = AttributeType.Attack,
                modifierType = ModifierType.Add,
                value = value,
                isPercent = isPercent
            };
        }
        
        /// <summary>
        /// 创建防御力修正
        /// </summary>
        public static AttributeModifierData CreateDefenseModifier(float value, bool isPercent = false)
        {
            return new AttributeModifierData
            {
                attributeType = AttributeType.Defense,
                modifierType = ModifierType.Add,
                value = value,
                isPercent = isPercent
            };
        }
        
        /// <summary>
        /// 创建速度修正
        /// </summary>
        public static AttributeModifierData CreateSpeedModifier(float value, bool isPercent = false)
        {
            return new AttributeModifierData
            {
                attributeType = AttributeType.Speed,
                modifierType = ModifierType.Add,
                value = value,
                isPercent = isPercent
            };
        }
        
        /// <summary>
        /// 创建暴击率修正
        /// </summary>
        public static AttributeModifierData CreateCritRateModifier(float value)
        {
            return new AttributeModifierData
            {
                attributeType = AttributeType.CriticalRate,
                modifierType = ModifierType.Add,
                value = value,
                isPercent = false
            };
        }
        
        #endregion
    }
}
