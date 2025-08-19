using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Battle.Skill
{
    /// <summary>
    /// 技能配置管理器
    /// 负责管理技能配置数据的加载和缓存
    /// </summary>
    public class SkillConfigManager
    {
        private static SkillConfigManager _instance;
        public static SkillConfigManager Instance => _instance ??= new SkillConfigManager();
        
        private Dictionary<int, SkillDefinition> _skillDefinitions;
        private Dictionary<string, SkillTemplate> _skillTemplates;
        
        private SkillConfigManager()
        {
            _skillDefinitions = new Dictionary<int, SkillDefinition>();
            _skillTemplates = new Dictionary<string, SkillTemplate>();
            InitializeSkillTemplates();
            LoadSkillDefinitions();
        }
        
        #region 技能模板系统
        
        /// <summary>
        /// 技能模板 - 用于快速创建常见技能类型
        /// </summary>
        public class SkillTemplate
        {
            public string templateName;
            public SkillType skillType;
            public List<EffectTemplate> effectTemplates;
            
            public SkillTemplate()
            {
                effectTemplates = new List<EffectTemplate>();
            }
        }
        
        public class EffectTemplate
        {
            public EffectType effectType;
            public TargetSelection targetSelection;
            public Dictionary<string, object> defaultParameters;
            
            public EffectTemplate()
            {
                defaultParameters = new Dictionary<string, object>();
            }
        }
        
        /// <summary>
        /// 初始化技能模板
        /// </summary>
        private void InitializeSkillTemplates()
        {
            // 基础攻击模板
            var basicAttackTemplate = new SkillTemplate
            {
                templateName = "BasicAttack",
                skillType = SkillType.Active,
                effectTemplates = new List<EffectTemplate>
                {
                    new EffectTemplate
                    {
                        effectType = EffectType.Damage,
                        targetSelection = TargetSelection.SingleEnemy,
                        defaultParameters = new Dictionary<string, object>
                        {
                            {"scaleAttribute", "attack"},
                            {"scaleValue", 1.0f},
                            {"canCrit", true}
                        }
                    }
                }
            };
            _skillTemplates["BasicAttack"] = basicAttackTemplate;
            
            // 群体攻击模板
            var aoeAttackTemplate = new SkillTemplate
            {
                templateName = "AOEAttack",
                skillType = SkillType.Active,
                effectTemplates = new List<EffectTemplate>
                {
                    new EffectTemplate
                    {
                        effectType = EffectType.Damage,
                        targetSelection = TargetSelection.AllEnemies,
                        defaultParameters = new Dictionary<string, object>
                        {
                            {"scaleAttribute", "attack"},
                            {"scaleValue", 0.8f},
                            {"canCrit", true}
                        }
                    }
                }
            };
            _skillTemplates["AOEAttack"] = aoeAttackTemplate;
            
            // 治疗模板
            var healTemplate = new SkillTemplate
            {
                templateName = "Heal",
                skillType = SkillType.Active,
                effectTemplates = new List<EffectTemplate>
                {
                    new EffectTemplate
                    {
                        effectType = EffectType.Heal,
                        targetSelection = TargetSelection.LowestHpAlly,
                        defaultParameters = new Dictionary<string, object>
                        {
                            {"scaleAttribute", "attack"},
                            {"scaleValue", 0.5f}
                        }
                    }
                }
            };
            _skillTemplates["Heal"] = healTemplate;
            
            // 增益Buff模板
            var buffTemplate = new SkillTemplate
            {
                templateName = "Buff",
                skillType = SkillType.Active,
                effectTemplates = new List<EffectTemplate>
                {
                    new EffectTemplate
                    {
                        effectType = EffectType.Buff,
                        targetSelection = TargetSelection.Self,
                        defaultParameters = new Dictionary<string, object>
                        {
                            {"duration", 3},
                            {"attackBonus", 10},
                            {"canStack", false}
                        }
                    }
                }
            };
            _skillTemplates["Buff"] = buffTemplate;
            
            // 减益Debuff模板
            var debuffTemplate = new SkillTemplate
            {
                templateName = "Debuff",
                skillType = SkillType.Active,
                effectTemplates = new List<EffectTemplate>
                {
                    new EffectTemplate
                    {
                        effectType = EffectType.Debuff,
                        targetSelection = TargetSelection.SingleEnemy,
                        defaultParameters = new Dictionary<string, object>
                        {
                            {"duration", 2},
                            {"attackReduction", 5},
                            {"canStack", false}
                        }
                    }
                }
            };
            _skillTemplates["Debuff"] = debuffTemplate;
            
            // 复合技能模板（伤害+Debuff）
            var damageDebuffTemplate = new SkillTemplate
            {
                templateName = "DamageDebuff",
                skillType = SkillType.Active,
                effectTemplates = new List<EffectTemplate>
                {
                    new EffectTemplate
                    {
                        effectType = EffectType.Damage,
                        targetSelection = TargetSelection.SingleEnemy,
                        defaultParameters = new Dictionary<string, object>
                        {
                            {"scaleAttribute", "attack"},
                            {"scaleValue", 1.2f},
                            {"canCrit", true}
                        }
                    },
                    new EffectTemplate
                    {
                        effectType = EffectType.Debuff,
                        targetSelection = TargetSelection.SingleEnemy,
                        defaultParameters = new Dictionary<string, object>
                        {
                            {"duration", 3},
                            {"defenseReduction", 10},
                            {"probability", 0.8f}
                        }
                    }
                }
            };
            _skillTemplates["DamageDebuff"] = damageDebuffTemplate;
        }
        
        #endregion
        
        #region 配置加载和管理
        
        /// <summary>
        /// 加载技能定义（从配置文件或数据库）
        /// </summary>
        private void LoadSkillDefinitions()
        {
            // 示例技能配置
            LoadExampleSkills();
        }
        
        /// <summary>
        /// 加载示例技能
        /// </summary>
        private void LoadExampleSkills()
        {
            // 1. 基础攻击
            var basicAttack = CreateSkillFromTemplate("BasicAttack", 1001, "基础攻击");
            basicAttack.cost = 0;
            basicAttack.cooldown = 0;
            basicAttack.description = "对单个敌人造成基于攻击力的伤害";
            _skillDefinitions[1001] = basicAttack;
            
            // 2. 火球术
            var fireball = CreateSkillFromTemplate("DamageDebuff", 1002, "火球术");
            fireball.cost = 20;
            fireball.cooldown = 2;
            fireball.description = "发射火球造成伤害，并有概率附加燃烧效果";
            
            // 修改火球术的效果参数
            fireball.effects[0].baseValue = 25; // 基础伤害25
            fireball.effects[1].parameters["duration"] = 3; // 燃烧持续3回合
            fireball.effects[1].parameters["tickDamage"] = 5; // 每回合5点燃烧伤害
            fireball.effects[1].parameters["specialType"] = "poison"; // 毒伤害类型
            
            _skillDefinitions[1002] = fireball;
            
            // 3. 治疗术
            var heal = CreateSkillFromTemplate("Heal", 1003, "治疗术");
            heal.cost = 15;
            heal.cooldown = 1;
            heal.description = "恢复血量最低盟友的生命值";
            heal.effects[0].baseValue = 30; // 基础治疗30
            _skillDefinitions[1003] = heal;
            
            // 4. 狂暴
            var rage = CreateSkillFromTemplate("Buff", 1004, "狂暴");
            rage.cost = 25;
            rage.cooldown = 3;
            rage.description = "提升自身攻击力，持续3回合";
            rage.effects[0].parameters["attackBonus"] = 15f;
            rage.effects[0].parameters["duration"] = 3;
            _skillDefinitions[1004] = rage;
            
            // 5. 闪电链（复杂技能示例）
            var lightningChain = new SkillDefinition
            {
                skillId = 1005,
                skillName = "闪电链",
                skillType = SkillType.Active,
                cost = 30,
                cooldown = 4,
                description = "发射闪电链，可以跳跃攻击多个敌人",
                castTime = 1.5f,
                effects = new List<SkillEffect>
                {
                    new SkillEffect
                    {
                        effectId = 1,
                        effectType = EffectType.Special,
                        targetSelection = TargetSelection.SingleEnemy,
                        baseValue = 35,
                        scaleAttribute = "attack",
                        scaleValue = 1.0f,
                        parameters = new Dictionary<string, object>
                        {
                            {"specialType", "chain"},
                            {"chainCount", 3},
                            {"damageReduction", 0.2f}
                        }
                    }
                }
            };
            _skillDefinitions[1005] = lightningChain;
            
            // 6. 吸血攻击
            var vampiricStrike = new SkillDefinition
            {
                skillId = 1006,
                skillName = "吸血攻击",
                skillType = SkillType.Active,
                cost = 20,
                cooldown = 3,
                description = "攻击敌人并恢复自身生命值",
                effects = new List<SkillEffect>
                {
                    new SkillEffect
                    {
                        effectType = EffectType.Special,
                        targetSelection = TargetSelection.SingleEnemy,
                        baseValue = 30,
                        scaleAttribute = "attack",
                        scaleValue = 1.2f,
                        parameters = new Dictionary<string, object>
                        {
                            {"specialType", "vampiric"},
                            {"vampiricRate", 0.4f}
                        }
                    }
                }
            };
            _skillDefinitions[1006] = vampiricStrike;
            
            // 7. 护盾术
            var shield = new SkillDefinition
            {
                skillId = 1007,
                skillName = "护盾术",
                skillType = SkillType.Active,
                cost = 25,
                cooldown = 4,
                description = "为盟友提供护盾，吸收伤害",
                effects = new List<SkillEffect>
                {
                    new SkillEffect
                    {
                        effectType = EffectType.Shield,
                        targetSelection = TargetSelection.SingleAlly,
                        baseValue = 50,
                        duration = 5,
                        parameters = new Dictionary<string, object>
                        {
                            {"shieldType", "magic"}
                        }
                    }
                }
            };
            _skillDefinitions[1007] = shield;
            
            Console.WriteLine($"[SkillConfigManager] 加载了 {_skillDefinitions.Count} 个技能定义");
        }
        
        /// <summary>
        /// 从模板创建技能
        /// </summary>
        private SkillDefinition CreateSkillFromTemplate(string templateName, int skillId, string skillName)
        {
            if (!_skillTemplates.ContainsKey(templateName))
            {
                throw new ArgumentException($"技能模板 {templateName} 不存在");
            }
            
            var template = _skillTemplates[templateName];
            var skillDef = new SkillDefinition
            {
                skillId = skillId,
                skillName = skillName,
                skillType = template.skillType,
                effects = new List<SkillEffect>()
            };
            
            foreach (var effectTemplate in template.effectTemplates)
            {
                var effect = new SkillEffect
                {
                    effectType = effectTemplate.effectType,
                    targetSelection = effectTemplate.targetSelection,
                    probability = 1.0f
                };
                
                // 复制默认参数
                foreach (var param in effectTemplate.defaultParameters)
                {
                    effect.parameters[param.Key] = param.Value;
                    
                    // 设置常用属性
                    switch (param.Key)
                    {
                        case "scaleAttribute":
                            effect.scaleAttribute = param.Value.ToString();
                            break;
                        case "scaleValue":
                            effect.scaleValue = Convert.ToSingle(param.Value);
                            break;
                        case "canCrit":
                            effect.canCrit = Convert.ToBoolean(param.Value);
                            break;
                        case "duration":
                            effect.duration = Convert.ToInt32(param.Value);
                            break;
                        case "canStack":
                            effect.canStack = Convert.ToBoolean(param.Value);
                            break;
                        case "probability":
                            effect.probability = Convert.ToSingle(param.Value);
                            break;
                    }
                }
                
                skillDef.effects.Add(effect);
            }
            
            return skillDef;
        }
        
        #endregion
        
        #region 公共接口
        
        /// <summary>
        /// 获取技能定义
        /// </summary>
        public SkillDefinition GetSkillDefinition(int skillId)
        {
            return _skillDefinitions.ContainsKey(skillId) ? _skillDefinitions[skillId] : null;
        }
        
        /// <summary>
        /// 获取所有技能定义
        /// </summary>
        public List<SkillDefinition> GetAllSkillDefinitions()
        {
            return _skillDefinitions.Values.ToList();
        }
        
        /// <summary>
        /// 根据类型获取技能
        /// </summary>
        public List<SkillDefinition> GetSkillsByType(SkillType skillType)
        {
            return _skillDefinitions.Values.Where(s => s.skillType == skillType).ToList();
        }
        
        /// <summary>
        /// 添加或更新技能定义
        /// </summary>
        public void AddOrUpdateSkillDefinition(SkillDefinition skillDef)
        {
            _skillDefinitions[skillDef.skillId] = skillDef;
        }
        
        /// <summary>
        /// 获取技能模板
        /// </summary>
        public SkillTemplate GetSkillTemplate(string templateName)
        {
            return _skillTemplates.ContainsKey(templateName) ? _skillTemplates[templateName] : null;
        }
        
        /// <summary>
        /// 添加技能模板
        /// </summary>
        public void AddSkillTemplate(string templateName, SkillTemplate template)
        {
            _skillTemplates[templateName] = template;
        }
        
        /// <summary>
        /// 验证技能配置
        /// </summary>
        public bool ValidateSkillDefinition(SkillDefinition skillDef)
        {
            if (skillDef == null) return false;
            if (skillDef.skillId <= 0) return false;
            if (string.IsNullOrEmpty(skillDef.skillName)) return false;
            if (skillDef.effects == null || skillDef.effects.Count == 0) return false;
            
            // 验证效果配置
            foreach (var effect in skillDef.effects)
            {
                if (effect.probability < 0 || effect.probability > 1) return false;
                if (effect.duration < 0) return false;
                if (effect.maxStacks < 0) return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 重新加载配置
        /// </summary>
        public void ReloadConfigurations()
        {
            _skillDefinitions.Clear();
            LoadSkillDefinitions();
        }
        
        #endregion
    }
}
