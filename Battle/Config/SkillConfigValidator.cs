using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Battle.Config
{
    /// <summary>
    /// 技能配置验证器
    /// 用于验证技能配置的合法性，确保配置数据正确
    /// </summary>
    public static class SkillConfigValidator
    {
        /// <summary>
        /// 验证技能配置数据
        /// </summary>
        /// <param name="skillConfig">要验证的技能配置</param>
        /// <returns>验证结果</returns>
        public static ValidationResult ValidateSkillConfig(SkillConfigData skillConfig)
        {
            var result = new ValidationResult();
            
            if (skillConfig == null)
            {
                result.AddError("技能配置不能为空");
                return result;
            }
            
            // 验证基础字段
            ValidateBasicFields(skillConfig, result);
            
            // 验证技能效果
            ValidateSkillEffects(skillConfig.effects, result);
            
            // 验证技能条件
            ValidateSkillConditions(skillConfig.conditions, result);
            
            // 验证AI配置
            ValidateAIConfig(skillConfig.aiConfig, result);
            
            return result;
        }
        
        /// <summary>
        /// 验证基础字段
        /// </summary>
        private static void ValidateBasicFields(SkillConfigData skillConfig, ValidationResult result)
        {
            if (skillConfig.skillId <= 0)
            {
                result.AddError("技能ID必须大于0");
            }
            
            if (string.IsNullOrEmpty(skillConfig.skillName))
            {
                result.AddError("技能名称不能为空");
            }
            
            if (skillConfig.cooldown < 0)
            {
                result.AddError("冷却时间不能小于0");
            }
            
            if (skillConfig.priority < 1 || skillConfig.priority > 10)
            {
                result.AddWarning("技能优先级建议设置在1-10之间");
            }
            
            if (skillConfig.maxTargets <= 0)
            {
                result.AddError("最大目标数量必须大于0");
            }
            
            // 验证消耗配置
            if (skillConfig.cost != null && skillConfig.cost.value < 0)
            {
                result.AddError("技能消耗值不能小于0");
            }
        }
        
        /// <summary>
        /// 验证技能效果
        /// </summary>
        private static void ValidateSkillEffects(List<SkillEffectData> effects, ValidationResult result)
        {
            if (effects == null || effects.Count == 0)
            {
                result.AddError("技能必须至少包含一个效果");
                return;
            }
            
            for (int i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                var prefix = $"效果[{i}]";
                
                if (effect.probability < 0 || effect.probability > 1)
                {
                    result.AddError($"{prefix}: 触发概率必须在0-1之间");
                }
                
                if (effect.duration < 0)
                {
                    result.AddError($"{prefix}: 持续时间不能小于0");
                }
                
                if (effect.maxStacks < 0)
                {
                    result.AddError($"{prefix}: 最大叠加层数不能小于0");
                }
                
                if (effect.delayRounds < 0)
                {
                    result.AddError($"{prefix}: 延迟回合数不能小于0");
                }
                
                // 验证属性修正
                ValidateAttributeModifiers(effect.attributeModifiers, result, prefix);
                
                // 验证特殊效果
                ValidateSpecialEffect(effect.specialEffect, result, prefix);
            }
        }
        
        /// <summary>
        /// 验证技能条件
        /// </summary>
        private static void ValidateSkillConditions(List<SkillConditionData> conditions, ValidationResult result)
        {
            if (conditions == null) return;
            
            for (int i = 0; i < conditions.Count; i++)
            {
                var condition = conditions[i];
                var prefix = $"条件[{i}]";
                
                if (condition.value < 0)
                {
                    result.AddWarning($"{prefix}: 条件值为负数，请确认是否正确");
                }
            }
        }
        
        /// <summary>
        /// 验证AI配置
        /// </summary>
        private static void ValidateAIConfig(AISkillConfig aiConfig, ValidationResult result)
        {
            if (aiConfig == null) return;
            
            if (aiConfig.selfHpThreshold < 0 || aiConfig.selfHpThreshold > 1)
            {
                result.AddError("AI自身血量阈值必须在0-1之间");
            }
            
            if (aiConfig.enemyHpThreshold < 0 || aiConfig.enemyHpThreshold > 1)
            {
                result.AddError("AI敌人血量阈值必须在0-1之间");
            }
            
            if (aiConfig.useProbability < 0 || aiConfig.useProbability > 1)
            {
                result.AddError("AI使用概率必须在0-1之间");
            }
            
            if (aiConfig.minEnemyCount < 0 || aiConfig.minEnemyCount > 6)
            {
                result.AddWarning("最少敌人数量建议设置在0-6之间");
            }
            
            if (aiConfig.minAllyCount < 0 || aiConfig.minAllyCount > 6)
            {
                result.AddWarning("最少队友数量建议设置在0-6之间");
            }
        }
        
        /// <summary>
        /// 验证属性修正
        /// </summary>
        private static void ValidateAttributeModifiers(AttributeModifierData[] modifiers, ValidationResult result, string prefix)
        {
            if (modifiers == null) return;
            
            for (int i = 0; i < modifiers.Length; i++)
            {
                var modifier = modifiers[i];
                var modifierPrefix = $"{prefix}.属性修正[{i}]";
                
                if (modifier.isPercent && (modifier.value < -1 || modifier.value > 10))
                {
                    result.AddWarning($"{modifierPrefix}: 百分比修正值建议在-100%到1000%之间");
                }
            }
        }
        
        /// <summary>
        /// 验证特殊效果
        /// </summary>
        private static void ValidateSpecialEffect(SpecialEffectData specialEffect, ValidationResult result, string prefix)
        {
            if (specialEffect == null) return;
            
            var effectPrefix = $"{prefix}.特殊效果";
            
            // 根据不同的特殊效果类型进行验证
            switch (specialEffect.effectType)
            {
                case SpecialEffectType.Chain:
                    ValidateChainEffect(specialEffect, result, effectPrefix);
                    break;
                    
                case SpecialEffectType.Vampiric:
                    ValidateVampiricEffect(specialEffect, result, effectPrefix);
                    break;
                    
                // 可以继续添加其他特殊效果的验证
            }
        }
        
        /// <summary>
        /// 验证连锁效果
        /// </summary>
        private static void ValidateChainEffect(SpecialEffectData effect, ValidationResult result, string prefix)
        {
            var chainCount = effect.GetIntParameter("chainCount", 1);
            var damageReduction = effect.GetFloatParameter("damageReduction", 0);
            
            if (chainCount <= 0 || chainCount > 10)
            {
                result.AddError($"{prefix}: 连锁次数必须在1-10之间");
            }
            
            if (damageReduction < 0 || damageReduction > 1)
            {
                result.AddError($"{prefix}: 伤害衰减比例必须在0-1之间");
            }
        }
        
        /// <summary>
        /// 验证吸血效果
        /// </summary>
        private static void ValidateVampiricEffect(SpecialEffectData effect, ValidationResult result, string prefix)
        {
            var vampiricRate = effect.GetFloatParameter("vampiricRate", 0);
            
            if (vampiricRate < 0 || vampiricRate > 2)
            {
                result.AddWarning($"{prefix}: 吸血比例建议在0-200%之间");
            }
        }
        
        /// <summary>
        /// 验证战斗单位配置
        /// </summary>
        public static ValidationResult ValidateUnitConfig(BattleUnitConfig unitConfig)
        {
            var result = new ValidationResult();
            
            if (unitConfig == null)
            {
                result.AddError("单位配置不能为空");
                return result;
            }
            
            if (unitConfig.unitId <= 0)
            {
                result.AddError("单位ID必须大于0");
            }
            
            if (string.IsNullOrEmpty(unitConfig.unitName))
            {
                result.AddError("单位名称不能为空");
            }
            
            if (unitConfig.level <= 0 || unitConfig.level > 100)
            {
                result.AddWarning("单位等级建议设置在1-100之间");
            }
            
            // 验证基础属性
            ValidateBaseAttributes(unitConfig.baseAttributes, result);
            
            // 验证AI配置
            ValidateUnitAIConfig(unitConfig.aiConfig, result);
            
            return result;
        }
        
        /// <summary>
        /// 验证基础属性
        /// </summary>
        private static void ValidateBaseAttributes(UnitBaseAttributes attributes, ValidationResult result)
        {
            if (attributes == null)
            {
                result.AddError("基础属性配置不能为空");
                return;
            }
            
            if (attributes.maxHp <= 0)
            {
                result.AddError("最大生命值必须大于0");
            }
            
            if (attributes.maxMp < 0)
            {
                result.AddError("最大法力值不能小于0");
            }
            
            if (attributes.attack < 0)
            {
                result.AddError("攻击力不能小于0");
            }
            
            if (attributes.defense < 0)
            {
                result.AddError("防御力不能小于0");
            }
            
            if (attributes.speed <= 0)
            {
                result.AddError("速度必须大于0");
            }
            
            if (attributes.critRate < 0 || attributes.critRate > 1)
            {
                result.AddError("暴击率必须在0-1之间");
            }
            
            if (attributes.critDamage < 1)
            {
                result.AddError("暴击伤害倍率不能小于1");
            }
            
            if (attributes.hitRate < 0 || attributes.hitRate > 1)
            {
                result.AddError("命中率必须在0-1之间");
            }
            
            if (attributes.dodgeRate < 0 || attributes.dodgeRate > 1)
            {
                result.AddError("闪避率必须在0-1之间");
            }
        }
        
        /// <summary>
        /// 验证单位AI配置
        /// </summary>
        private static void ValidateUnitAIConfig(UnitAIConfig aiConfig, ValidationResult result)
        {
            if (aiConfig == null) return;
            
            var totalWeight = aiConfig.aggressionWeight + aiConfig.defenseWeight + aiConfig.supportWeight;
            
            if (totalWeight <= 0)
            {
                result.AddError("AI权重总和必须大于0");
            }
            
            if (Math.Abs(totalWeight - 1.0f) > 0.01f)
            {
                result.AddWarning("AI权重总和建议等于1.0");
            }
        }
    }
    
    /// <summary>
    /// 验证结果类
    /// 包含验证过程中发现的错误和警告信息
    /// </summary>
    public class ValidationResult
    {
        public List<string> Errors { get; private set; }
        public List<string> Warnings { get; private set; }
        
        public bool IsValid => Errors.Count == 0;
        public bool HasWarnings => Warnings.Count > 0;
        
        public ValidationResult()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
        }
        
        public void AddError(string error)
        {
            Errors.Add(error);
        }
        
        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }
        
        public string GetSummary()
        {
            var summary = $"验证结果: {(IsValid ? "通过" : "失败")}";
            
            if (Errors.Count > 0)
            {
                summary += $"\n错误({Errors.Count}个):";
                foreach (var error in Errors)
                {
                    summary += $"\n  - {error}";
                }
            }
            
            if (Warnings.Count > 0)
            {
                summary += $"\n警告({Warnings.Count}个):";
                foreach (var warning in Warnings)
                {
                    summary += $"\n  - {warning}";
                }
            }
            
            return summary;
        }
    }
}
