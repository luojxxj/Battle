using Battle.Enum;

namespace Server.Battle.Config
{
    /// <summary>
    /// 技能模板系统
    /// 提供预定义的技能模板，方便策划快速创建常见技能
    /// </summary>
    public static class SkillTemplates
    {
        /// <summary>
        /// 创建技能
        /// </summary>
        /// <param name="skillId">技能Id</param>
        /// <param name="skillType">技能类型</param>
        /// <param name="revolutionType">释放条件类型</param>
        /// <param name="revolutionCost">释放条件值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="targetCount">范围</param>
        /// <param name="effectList">效果列表</param>
        /// <param name="priority"></param>
        /// <param name="critical"></param>
        /// <param name="hit"></param>
        /// <returns>创建技能</returns>
        public static SkillConfigData CreateSkill(int skillId, SkillType skillType, RevolutionType revolutionType, int revolutionCost, TargetType targetType, int targetCount, List<EffectData>effectList, int priority, bool critical, bool hit)
        {
            return new SkillConfigData
            {
                skillId = skillId,
                skillType = SkillType.Active,
                revolutionCost = 0,
                priority = 1,
                targetType = TargetType.SingleEnemy,
                targetCount = 1,
                effects = new List<EffectData>
                {
                    new EffectData
                    {
                        effectType = EffectType.Damage,
                        param = new List<int> { 1,1,1,1 },
                    }
                },
            };
        }
        
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
