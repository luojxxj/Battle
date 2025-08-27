
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

            
            if (skillConfig.revolutionCost < 0)
            {
                result.AddError("冷却时间不能小于0");
            }
            
            if (skillConfig.priority < 1 || skillConfig.priority > 10)
            {
                result.AddWarning("技能优先级建议设置在1-10之间");
            }
            
            if (skillConfig.targetCount <= 0)
            {
                result.AddError("最大目标数量必须大于0");
            }
        }
        
        /// <summary>
        /// 验证技能效果
        /// </summary>
        private static void ValidateSkillEffects(List<EffectData> effects, ValidationResult result)
        {
            if (effects == null || effects.Count == 0)
            {
                result.AddError("技能必须至少包含一个效果");
                return;
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
