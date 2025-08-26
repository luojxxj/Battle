using Battle.Enum;
using Server.Battle.Core;
using static Server.Battle.Data.ServerBattleData;

namespace Server.Battle.Skill
{
    /// <summary>
    /// 技能执行引擎
    /// 负责处理技能的执行逻辑和效果计算
    /// </summary>
    public class SkillExecutionEngine
    {
        private BuffManager _buffManager;
        private Random _random;
        private Dictionary<int, SkillDefinition> _skillDefinitions;
        private BattleCalculator _battleCalculator;

        public SkillExecutionEngine(BattleCalculator battleCalculator)
        {
            _buffManager = new BuffManager();
            _random = new Random();
            _skillDefinitions = new Dictionary<int, SkillDefinition>();
            _battleCalculator = battleCalculator;
        }
        
        #region 技能执行核心方法
        
        /// <summary>
        /// 执行技能
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="skillId">技能ID</param>
        /// <param name="targets">目标列表</param>
        /// <returns>技能执行结果</returns>
        public SkillExecutionResult ExecuteSkill(
            BattleUnit caster, 
            int skillId, 
            List<BattleUnit> targets)
        {
            var result = new SkillExecutionResult
            {
                skillId = skillId,
                casterId = caster.unitId,
                success = false,
                actions = new List<BattleAction>()
            };
            
            // 获取技能定义
            if (!_skillDefinitions.ContainsKey(skillId))
            {
                result.errorMessage = $"技能ID {skillId} 不存在";
                return result;
            }
            
            var skillDef = _skillDefinitions[skillId];
            Console.WriteLine($"[SkillExecutionEngine] Executing skill {skillDef.skillName} ({skillId}) for {caster.unitName}");
            
            // 执行技能效果
            foreach (var effect in skillDef.effects)
            {
                var effectTargets = _battleCalculator.SelectTargetsBySelection(caster, effect.targetType, targets);
                var effectActions = ExecuteSkillEffect(caster, effect, effectTargets);
                result.actions.AddRange(effectActions);
            }
            
            result.success = true;
            return result;
        }
        
        /// <summary>
        /// 执行单个技能效果
        /// </summary>
        private List<BattleAction> ExecuteSkillEffect(
            BattleUnit caster,
            SkillEffect effect,
            List<BattleUnit> targets)
        {
            var actions = new List<BattleAction>();
            
            foreach (var target in targets)
            {
                // 概率判定
                if (_random.NextDouble() > effect.probability)
                {
                    continue; // 未触发
                }
                
                var action = CreateActionFromEffect(caster, target, effect);
                
                switch (effect.effectType)
                {
                    case EffectType.Damage:
                        ExecuteDamageEffect(caster, target, effect, action);
                        break;
                        
                    case EffectType.Heal:
                        ExecuteHealEffect(caster, target, effect, action);
                        break;
                        
                    case EffectType.AddBuff:
                        ExecuteBuffEffect(caster, target, effect, action);
                        break;
                        
                    case EffectType.Shield:
                        ExecuteShieldEffect(caster, target, effect, action);
                        break;
                        
                    case EffectType.Revive:
                        ExecuteDispelEffect(caster, target, effect, action);
                        break;
                }
                
                actions.Add(action);
            }
            
            return actions;
        }
        
        #endregion
        
        #region 效果执行方法
        
        /// <summary>
        /// 执行伤害效果
        /// </summary>
        private void ExecuteDamageEffect(
            BattleUnit caster,
            BattleUnit target,
            SkillEffect effect,
            BattleAction action)
        {
            // 计算基础伤害
            int baseDamage = CalculateEffectValue(caster, effect);
            
            // 应用防御
            int finalDamage = baseDamage;
            if (!effect.ignoreDefense)
            {
                int defense = GetUnitFinalAttribute(target, "defense");
                finalDamage = Math.Max(1, baseDamage - defense / 2);
            }
            
            // 暴击判定
            if (effect.canCrit && _random.NextDouble() < 0.2f) // 20%暴击率
            {
                finalDamage = (int)(finalDamage * 1.5f);
                action.isCritical = true;
            }
            
            // 应用伤害
            target.currentHp = Math.Max(0, target.currentHp - finalDamage);
            action.actualValue = finalDamage;
            
            // 更新统计
            caster.totalDamageDealt += finalDamage;
            target.totalDamageReceived += finalDamage;
            
            Console.WriteLine($"[SkillEngine] {caster.unitName} 使用技能对 {target.unitName} 造成 {finalDamage} 点伤害");
        }
        
        /// <summary>
        /// 执行治疗效果
        /// </summary>
        private void ExecuteHealEffect(
            BattleUnit caster,
            BattleUnit target,
            SkillEffect effect,
            BattleAction action)
        {
            int healAmount = CalculateEffectValue(caster, effect);
            int actualHeal = Math.Min(healAmount, target.maxHp - target.currentHp);
            
            target.currentHp += actualHeal;
            action.actualValue = actualHeal;
            
            Console.WriteLine($"[SkillEngine] {caster.unitName} 为 {target.unitName} 恢复 {actualHeal} 点生命值");
        }
        
        /// <summary>
        /// 执行Buff效果
        /// </summary>
        private void ExecuteBuffEffect(
            BattleUnit caster,
            BattleUnit target,
            SkillEffect effect,
            BattleAction action)
        {
            var buff = _buffManager.AddBuff(target.unitId, effect, caster.unitId, action.skillId);
            action.actualValue = effect.baseValue;
            
            string effectName = effect.effectType == EffectType.AddBuff ? "增益" : "减益";
            Console.WriteLine($"[SkillEngine] {caster.unitName} 为 {target.unitName} 施加 {effectName} 效果");
        }
        
        /// <summary>
        /// 执行护盾效果
        /// </summary>
        private void ExecuteShieldEffect(
            BattleUnit caster,
            BattleUnit target,
            SkillEffect effect,
            BattleAction action)
        {
            int shieldAmount = CalculateEffectValue(caster, effect);
            
            // 创建护盾Buff
            var shieldEffect = new SkillEffect
            {
                effectType = EffectType.Shield,
                baseValue = shieldAmount,
                duration = effect.duration
            };
            
            shieldEffect.parameters["shieldAmount"] = shieldAmount;
            
            _buffManager.AddBuff(target.unitId, shieldEffect, caster.unitId, action.skillId);
            action.actualValue = shieldAmount;
            
            Console.WriteLine($"[SkillEngine] {caster.unitName} 为 {target.unitName} 提供 {shieldAmount} 点护盾");
        }
        
        /// <summary>
        /// 执行驱散效果
        /// </summary>
        private void ExecuteDispelEffect(
            BattleUnit caster,
            BattleUnit target,
            SkillEffect effect,
            BattleAction action)
        {
            var targetBuffs = _buffManager.GetUnitBuffs(target.unitId);
            int dispelCount = 0;
            
            // 根据参数决定驱散类型
            bool dispelPositive = effect.parameters.ContainsKey("dispelPositive") && 
                                 Convert.ToBoolean(effect.parameters["dispelPositive"]);
            bool dispelNegative = effect.parameters.ContainsKey("dispelNegative") && 
                                 Convert.ToBoolean(effect.parameters["dispelNegative"]);
            
            foreach (var buff in targetBuffs.ToArray()) // 使用ToArray避免修改集合时的异常
            {
                bool shouldDispel = false;
                
                if (dispelPositive && buff.buffType == EffectType.AddBuff)
                {
                    shouldDispel = true;
                }
                else if (dispelNegative && buff.buffType == EffectType.RemoveBuff)
                {
                    shouldDispel = true;
                }
                
                if (shouldDispel)
                {
                    _buffManager.RemoveBuff(target.unitId, buff.buffId);
                    dispelCount++;
                }
            }
            
            action.actualValue = dispelCount;
            Console.WriteLine($"[SkillEngine] {caster.unitName} 驱散了 {target.unitName} 的 {dispelCount} 个效果");
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 计算效果数值
        /// </summary>
        private int CalculateEffectValue(BattleUnit caster, SkillEffect effect)
        {
            int baseValue = effect.baseValue;
            
            // 添加属性缩放
            if (!string.IsNullOrEmpty(effect.scaleAttribute))
            {
                int attributeValue = GetUnitFinalAttribute(caster, effect.scaleAttribute);
                baseValue += (int)(attributeValue * effect.scaleValue);
            }
            
            return baseValue;
        }
        
        /// <summary>
        /// 获取单位的最终属性值（包含Buff修正）
        /// </summary>
        private int GetUnitFinalAttribute(BattleUnit unit, string attributeName)
        {
            int baseValue = GetUnitBaseAttribute(unit, attributeName);
            float modifier = 0f;
            
            // 应用Buff修正
            var buffs = _buffManager.GetUnitBuffs(unit.unitId);
            foreach (var buff in buffs)
            {
                if (buff.attributeModifiers.ContainsKey(attributeName))
                {
                    modifier += buff.attributeModifiers[attributeName] * buff.currentStacks;
                }
            }
            
            return Math.Max(0, baseValue + (int)modifier);
        }
        
        /// <summary>
        /// 获取单位基础属性
        /// </summary>
        private int GetUnitBaseAttribute(BattleUnit unit, string attributeName)
        {
            switch (attributeName)
            {
                case "attack": return unit.attack;
                case "defense": return unit.defense;
                case "speed": return unit.speed;
                case "maxHp": return unit.maxHp;
                case "currentHp": return unit.currentHp;
                default: return 0;
            }
        }
        
        /// <summary>
        /// 选择目标
        /// </summary>
        
        
        /// <summary>
        /// 比较数值
        /// </summary>
        private bool CompareValues(float actual, string comparison, float expected)
        {
            switch (comparison)
            {
                case ">": return actual > expected;
                case "<": return actual < expected;
                case ">=": return actual >= expected;
                case "<=": return actual <= expected;
                case "==": return Math.Abs(actual - expected) < 0.001f;
                case "!=": return Math.Abs(actual - expected) >= 0.001f;
                default: return false;
            }
        }
        
        /// <summary>
        /// 从效果创建动作
        /// </summary>
        private BattleAction CreateActionFromEffect(
            BattleUnit caster,
            BattleUnit target,
            SkillEffect effect)
        {
            return new BattleAction
            {
                actionType = ConvertEffectTypeToActionType(effect.effectType),
                sourceUnitId = caster.unitId,
                targetUnitIds = new List<long> { target.unitId },
                value = effect.baseValue,
                actualValue = effect.baseValue,
                description = $"{caster.unitName} 对 {target.unitName} 使用技能效果"
            };
        }
        
        /// <summary>
        /// 转换效果类型到动作类型
        /// </summary>
        private ActionType ConvertEffectTypeToActionType(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.Damage: return ActionType.Damage;
                case EffectType.Heal: return ActionType.Heal;
                case EffectType.AddBuff: return ActionType.AddBuff;
                case EffectType.Trigger: return ActionType.Trigger;
                case EffectType.Shield: return ActionType.Shield;
                case EffectType.Revive: return ActionType.Revive;
                case EffectType.Dispel: return ActionType.Dispel;
                default: return ActionType.Damage;
            }
        }
        
        #endregion
        
        #region 公共管理方法
        
        /// <summary>
        /// 注册技能定义
        /// </summary>
        public void RegisterSkillDefinition(SkillDefinition skillDef)
        {
            _skillDefinitions[skillDef.skillId] = skillDef;
        }
        
        /// <summary>
        /// 批量注册技能定义
        /// </summary>
        public void RegisterSkillDefinitions(List<SkillDefinition> skillDefs)
        {
            foreach (var skillDef in skillDefs)
            {
                RegisterSkillDefinition(skillDef);
            }
        }
        
        /// <summary>
        /// 更新所有Buff（每回合调用）
        /// </summary>
        public void UpdateAllBuffs(List<BattleUnit> allUnits, int currentRound)
        {
            foreach (var unit in allUnits)
            {
                _buffManager.UpdateBuffs(unit.unitId, currentRound);
            }
        }
        
        /// <summary>
        /// 获取单位的Buff管理器
        /// </summary>
        public BuffManager GetBuffManager()
        {
            return _buffManager;
        }
        
        #endregion
    }
    
    /// <summary>
    /// 技能执行结果
    /// </summary>
    public class SkillExecutionResult
    {
        public int skillId;                                    // 技能ID
        public long casterId;                                   // 施法者ID
        public bool success;                                   // 是否成功
        public string errorMessage;                            // 错误消息
        public List<BattleAction> actions;   // 产生的动作列表
        
        public SkillExecutionResult()
        {
            actions = new List<BattleAction>();
        }
    }
}
