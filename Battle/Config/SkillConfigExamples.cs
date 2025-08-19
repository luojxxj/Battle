using System.Collections.Generic;
using Server.Battle.Config;
using Server.Battle.Skill;

namespace Server.Battle.Example
{
    /// <summary>
    /// 技能配置示例
    /// 展示如何使用技能模板系统创建各种类型的技能
    /// </summary>
    public static class SkillConfigExamples
    {
        /// <summary>
        /// 获取所有示例技能配置
        /// </summary>
        /// <returns>技能配置列表</returns>
        public static List<SkillConfigData> GetAllExampleSkills()
        {
            var skills = new List<SkillConfigData>();
            
            // 基础攻击技能
            skills.Add(CreateBasicAttackExample());
            
            // 战士技能
            skills.AddRange(CreateWarriorSkills());
            
            // 法师技能
            skills.AddRange(CreateMageSkills());
            
            // 坦克技能
            skills.AddRange(CreateTankSkills());
            
            // 辅助技能
            skills.AddRange(CreateSupportSkills());
            
            // 刺客技能
            skills.AddRange(CreateAssassinSkills());
            
            return skills;
        }
        
        #region 基础技能
        
        /// <summary>
        /// 基础攻击示例
        /// </summary>
        private static SkillConfigData CreateBasicAttackExample()
        {
            return SkillTemplates.CreateBasicAttack(1001, "基础攻击", 1.0f);
        }
        
        #endregion
        
        #region 战士技能
        
        /// <summary>
        /// 创建战士职业技能
        /// </summary>
        private static List<SkillConfigData> CreateWarriorSkills()
        {
            var skills = new List<SkillConfigData>();
            
            // 强力攻击
            var powerStrike = SkillTemplates.CreateBasicAttack(2001, "强力攻击", 1.5f);
            powerStrike.cost = new CostData { type = CostType.Mp, value = 15 };
            powerStrike.cooldown = 2;
            powerStrike.priority = 3;
            powerStrike.effects[0].baseValue = 10;
            skills.Add(powerStrike);
            
            // 横扫攻击
            var sweep = SkillTemplates.CreateAOEAttack(2002, "横扫", 8, 0.8f, 25, 3, ElementType.Physical);
            skills.Add(sweep);
            
            // 战斗怒吼（团队Buff）
            var battleCry = SkillTemplates.CreateBuffSkill(
                2003, 
                "战斗怒吼", 
                "battle_cry_buff", 
                4, 
                30, 
                TargetType.AllAllies,
                new AttributeModifierData[]
                {
                    SkillTemplates.CreateAttackModifier(5),
                    SkillTemplates.CreateCritRateModifier(0.1f)
                }
            );
            skills.Add(battleCry);
            
            // 挑衅（强制敌人攻击自己）
            var taunt = SkillTemplates.CreateDebuffSkill(
                2004,
                "挑衅",
                "taunt_debuff",
                3,
                0.9f,
                20,
                new AttributeModifierData[]
                {
                    SkillTemplates.CreateAttackModifier(-3)
                }
            );
            skills.Add(taunt);
            
            return skills;
        }
        
        #endregion
        
        #region 法师技能
        
        /// <summary>
        /// 创建法师职业技能
        /// </summary>
        private static List<SkillConfigData> CreateMageSkills()
        {
            var skills = new List<SkillConfigData>();
            
            // 火球术
            var fireball = SkillTemplates.CreateBasicAttack(3001, "火球术", 1.2f);
            fireball.cost = new CostData { type = CostType.Mp, value = 20 };
            fireball.cooldown = 1;
            fireball.effects[0].damageType = DamageType.Magical;
            fireball.effects[0].scaleType = AttributeType.Attack;
            fireball.effects[0].element = ElementType.Fire;
            fireball.effects[0].baseValue = 15;
            skills.Add(fireball);
            
            // 暴风雪（AOE冰系攻击）
            var blizzard = SkillTemplates.CreateAOEAttack(3002, "暴风雪", 12, 1.0f, 40, 4, ElementType.Water);
            skills.Add(blizzard);
            
            // 闪电链
            var lightningChain = SkillTemplates.CreateChainAttack(3003, "闪电链", 18, 3, 0.25f, 35);
            skills.Add(lightningChain);
            
            // 毒云术（DOT技能）
            var poisonCloud = SkillTemplates.CreateDOTSkill(3004, "毒云术", 10, 4, 5, ElementType.Earth, 30);
            skills.Add(poisonCloud);
            
            // 法师护盾
            var mageShield = SkillTemplates.CreateShieldSkill(3005, "法师护盾", 25, 5, 25, TargetType.Self);
            skills.Add(mageShield);
            
            return skills;
        }
        
        #endregion
        
        #region 坦克技能
        
        /// <summary>
        /// 创建坦克职业技能
        /// </summary>
        private static List<SkillConfigData> CreateTankSkills()
        {
            var skills = new List<SkillConfigData>();
            
            // 护盾墙（自身超强护盾）
            var shieldWall = SkillTemplates.CreateShieldSkill(4001, "护盾墙", 50, 4, 35, TargetType.Self);
            skills.Add(shieldWall);
            
            // 团队护盾
            var teamShield = SkillTemplates.CreateShieldSkill(4002, "团队护盾", 20, 3, 50, TargetType.AllAllies);
            skills.Add(teamShield);
            
            // 嘲讽（减少敌人攻击力）
            var intimidate = SkillTemplates.CreateDebuffSkill(
                4003,
                "威吓",
                "intimidate_debuff",
                4,
                0.8f,
                25,
                new AttributeModifierData[]
                {
                    SkillTemplates.CreateAttackModifier(-8),
                    SkillTemplates.CreateCritRateModifier(-0.15f)
                }
            );
            skills.Add(intimidate);
            
            // 反击姿态（被动技能转主动）
            var counterStance = SkillTemplates.CreateBuffSkill(
                4004,
                "反击姿态",
                "counter_stance_buff",
                5,
                40,
                TargetType.Self,
                new AttributeModifierData[]
                {
                    SkillTemplates.CreateDefenseModifier(10),
                    // 添加反击效果的特殊参数
                }
            );
            skills.Add(counterStance);
            
            return skills;
        }
        
        #endregion
        
        #region 辅助技能
        
        /// <summary>
        /// 创建辅助职业技能
        /// </summary>
        private static List<SkillConfigData> CreateSupportSkills()
        {
            var skills = new List<SkillConfigData>();
            
            // 治愈术（单体治疗）
            var heal = SkillTemplates.CreateHealSkill(5001, "治愈术", 20, 25, 1, TargetType.AllyLowestHp);
            skills.Add(heal);
            
            // 群体治疗
            var massHeal = SkillTemplates.CreateHealSkill(5002, "群体治疗", 12, 45, 3, TargetType.AllAllies);
            skills.Add(massHeal);
            
            // 祝福（提升团队属性）
            var blessing = SkillTemplates.CreateBuffSkill(
                5003,
                "祝福",
                "blessing_buff",
                6,
                35,
                TargetType.AllAllies,
                new AttributeModifierData[]
                {
                    SkillTemplates.CreateAttackModifier(3),
                    SkillTemplates.CreateDefenseModifier(3),
                    SkillTemplates.CreateSpeedModifier(2)
                }
            );
            skills.Add(blessing);
            
            // 驱散（移除Debuff）
            var dispel = new SkillConfigData
            {
                skillId = 5004,
                skillName = "驱散",
                description = "移除友方单位的负面状态",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = 20 },
                cooldown = 2,
                priority = 9,
                targetType = TargetType.SingleAlly,
                maxTargets = 1,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Dispel,
                        probability = 1.0f,
                        baseValue = 0,
                        duration = 0,
                        effectRange = 1,
                        delayRounds = 0
                    }
                },
                aiConfig = new AISkillConfig
                {
                    selfHpThreshold = 1.0f,
                    enemyHpThreshold = 1.0f,
                    minEnemyCount = 0,
                    minAllyCount = 1,
                    useProbability = 0.95f,
                    priorityModifier = 3.0f,
                    targetPreference = TargetPreference.Weakest
                }
            };
            skills.Add(dispel);
            
            // 复活术
            var resurrect = new SkillConfigData
            {
                skillId = 5005,
                skillName = "复活术",
                description = "复活死亡的队友",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = 80 },
                cooldown = 8,
                priority = 10,
                targetType = TargetType.SingleAlly,
                maxTargets = 1,
                conditions = new List<SkillConditionData>
                {
                    new SkillConditionData
                    {
                        conditionType = ConditionType.AllyCount,
                        comparison = ComparisonType.Less,
                        value = 6,
                        targetTag = "dead_ally"
                    }
                },
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Special,
                        probability = 1.0f,
                        baseValue = 30, // 复活时的血量百分比
                        specialEffect = new SpecialEffectData
                        {
                            effectType = SpecialEffectType.Revive
                        },
                        effectRange = 1,
                        delayRounds = 0
                    }
                },
                aiConfig = new AISkillConfig
                {
                    selfHpThreshold = 0.5f,
                    enemyHpThreshold = 1.0f,
                    minEnemyCount = 0,
                    minAllyCount = 0,
                    useProbability = 1.0f,
                    priorityModifier = 5.0f,
                    targetPreference = TargetPreference.Weakest
                }
            };
            skills.Add(resurrect);
            
            return skills;
        }
        
        #endregion
        
        #region 刺客技能
        
        /// <summary>
        /// 创建刺客职业技能
        /// </summary>
        private static List<SkillConfigData> CreateAssassinSkills()
        {
            var skills = new List<SkillConfigData>();
            
            // 背刺（高暴击攻击）
            var backstab = SkillTemplates.CreateBasicAttack(6001, "背刺", 1.3f);
            backstab.cost = new CostData { type = CostType.Mp, value = 20 };
            backstab.cooldown = 2;
            backstab.priority = 4;
            backstab.effects[0].baseValue = 8;
            backstab.effects[0].canCrit = true;
            // 为背刺添加额外暴击率
            backstab.effects.Add(new SkillEffectData
            {
                effectType = EffectType.Buff,
                probability = 1.0f,
                buffId = "backstab_crit_buff",
                duration = 1,
                attributeModifiers = new AttributeModifierData[]
                {
                    SkillTemplates.CreateCritRateModifier(0.5f) // +50%暴击率
                }
            });
            skills.Add(backstab);
            
            // 毒刃攻击
            var poisonBlade = SkillTemplates.CreateDOTSkill(6002, "毒刃", 12, 3, 4, ElementType.Dark, 25);
            skills.Add(poisonBlade);
            
            // 影分身（创建自身副本）
            var shadowClone = new SkillConfigData
            {
                skillId = 6003,
                skillName = "影分身",
                description = "创建一个影分身协助战斗",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = 60 },
                cooldown = 6,
                priority = 6,
                targetType = TargetType.Self,
                maxTargets = 1,
                conditions = new List<SkillConditionData>(),
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Summon,
                        probability = 1.0f,
                        duration = 5,
                        baseValue = 50, // 影分身血量为本体的50%
                        specialEffect = new SpecialEffectData
                        {
                            effectType = SpecialEffectType.Copy
                        },
                        effectRange = 1,
                        delayRounds = 0
                    }
                },
                aiConfig = new AISkillConfig
                {
                    selfHpThreshold = 0.6f,
                    enemyHpThreshold = 1.0f,
                    minEnemyCount = 2,
                    minAllyCount = 0,
                    useProbability = 0.8f,
                    priorityModifier = 1.5f,
                    targetPreference = TargetPreference.Random
                }
            };
            skills.Add(shadowClone);
            
            // 隐身（暂时无法被选中）
            var stealth = SkillTemplates.CreateBuffSkill(
                6004,
                "隐身",
                "stealth_buff",
                3,
                35,
                TargetType.Self,
                new AttributeModifierData[]
                {
                    SkillTemplates.CreateSpeedModifier(5),
                    SkillTemplates.CreateCritRateModifier(0.2f)
                }
            );
            // 添加隐身的特殊效果
            stealth.effects[0].specialEffect = new SpecialEffectData
            {
                effectType = SpecialEffectType.Transform
            };
            stealth.effects[0].specialEffect.boolParameters["untargetable"] = true;
            skills.Add(stealth);
            
            // 致命一击（处决技能）
            var executeStrike = new SkillConfigData
            {
                skillId = 6005,
                skillName = "致命一击",
                description = "对血量低的敌人造成大量伤害",
                skillType = SkillType.Active,
                cost = new CostData { type = CostType.Mp, value = 40 },
                cooldown = 4,
                priority = 2,
                targetType = TargetType.EnemyLowestCurrentHp,
                maxTargets = 1,
                conditions = new List<SkillConditionData>
                {
                    new SkillConditionData
                    {
                        conditionType = ConditionType.EnemyHp,
                        comparison = ComparisonType.Less,
                        value = 0.3f // 敌人血量低于30%
                    }
                },
                effects = new List<SkillEffectData>
                {
                    new SkillEffectData
                    {
                        effectType = EffectType.Damage,
                        damageType = DamageType.Physical,
                        probability = 1.0f,
                        baseValue = 30,
                        scaleType = AttributeType.Attack,
                        scaleValue = 2.0f, // 高倍率
                        canCrit = true,
                        element = ElementType.Dark,
                        ignoreDefense = true, // 无视防御
                        effectRange = 1,
                        delayRounds = 0
                    }
                },
                aiConfig = new AISkillConfig
                {
                    selfHpThreshold = 1.0f,
                    enemyHpThreshold = 0.3f,
                    minEnemyCount = 1,
                    minAllyCount = 0,
                    useProbability = 1.0f,
                    priorityModifier = 4.0f,
                    targetPreference = TargetPreference.LowestHp
                }
            };
            skills.Add(executeStrike);
            
            return skills;
        }
        
        #endregion
        
        #region 验证示例
        
        /// <summary>
        /// 验证所有示例技能配置
        /// </summary>
        public static void ValidateAllExampleSkills()
        {
            var skills = GetAllExampleSkills();
            var totalErrors = 0;
            var totalWarnings = 0;
            
            Console.WriteLine("=== 技能配置验证报告 ===");
            
            foreach (var skill in skills)
            {
                var result = SkillConfigValidator.ValidateSkillConfig(skill);
                
                if (!result.IsValid || result.HasWarnings)
                {
                    Console.WriteLine($"\n技能: {skill.skillName} (ID: {skill.skillId})");
                    Console.WriteLine(result.GetSummary());
                    
                    totalErrors += result.Errors.Count;
                    totalWarnings += result.Warnings.Count;
                }
            }
            
            Console.WriteLine($"\n=== 验证总结 ===");
            Console.WriteLine($"总技能数: {skills.Count}");
            Console.WriteLine($"总错误数: {totalErrors}");
            Console.WriteLine($"总警告数: {totalWarnings}");
            Console.WriteLine($"验证通过率: {((skills.Count - totalErrors) * 100.0 / skills.Count):F1}%");
        }
        
        #endregion
    }
}
