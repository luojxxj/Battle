using Battle.Enum;
using Server.Battle.Core;
using Server.Battle.Skill;
using static Server.Battle.Data.ServerBattleData;

namespace Server.Battle.Example
{
    /// <summary>
    /// 技能系统使用示例
    /// 展示如何使用新的技能框架进行战斗计算
    /// </summary>
    public class SkillSystemExample
    {
        public static void RunExample()
        {
            Console.WriteLine("=== 技能系统示例 ===");
            
            // 1. 创建技能系统实例
            var skillConfig = SkillConfigManager.Instance;
            var battleCalculator = new BattleCalculator();
            
            Console.WriteLine($"已加载 {skillConfig.GetAllSkillDefinitions().Count} 个技能定义");
            
            // 2. 创建测试单位
            var playerUnit = CreateTestUnit(1, "战士");
            var enemyUnit = CreateTestUnit(2, "哥布林");
            
            // 3. 为单位分配技能
            var playerSkills = new List<int> { 1001, 1002, 1004, 1006 }; // 基础攻击、火球术、狂暴、吸血攻击
            var enemySkills = new List<int> { 1001, 1003 }; // 基础攻击、治疗术
            
            battleCalculator.InitializeUnitSkills(playerUnit.unitId, playerSkills);
            battleCalculator.InitializeUnitSkills(enemyUnit.unitId, enemySkills);
            
            // 4. 显示技能信息
            DisplaySkillInfo(playerSkills, "玩家技能");
            DisplaySkillInfo(enemySkills, "敌人技能");
            
            // 5. 模拟技能使用
            SimulateSkillUsage(battleCalculator, playerUnit, enemyUnit);
            
            // 6. 演示Buff系统
            DemonstrateBuffSystem(battleCalculator.GetSkillEngine());
            
            // 7. 演示复合技能
            DemonstrateComplexSkill();
            
            Console.WriteLine("=== 示例完成 ===");
        }
        
        /// <summary>
        /// 创建测试单位
        /// </summary>
        private static BattleUnit CreateTestUnit(int id, string name)
        {
            return new BattleUnit
            {
                unitId = id,
                unitName = name,
                maxHp = 100,
                currentHp = 100,
                attack = 25,
                defense = 10,
                speed = 15,
                isAlive = true,
            };
        }
        
        /// <summary>
        /// 显示技能信息
        /// </summary>
        private static void DisplaySkillInfo(List<int> skillIds, string category)
        {
            Console.WriteLine($"\n--- {category} ---");
            var skillConfig = SkillConfigManager.Instance;
            
            foreach (var skillId in skillIds)
            {
                var skill = skillConfig.GetSkillDefinition(skillId);
                if (skill != null)
                {
                    Console.WriteLine($"技能: {skill.skillName} (ID: {skill.skillId})");
                    Console.WriteLine($"  类型: {skill.skillType}, 消耗: {skill.cost}, 冷却: {skill.cooldown}");
                    Console.WriteLine($"  描述: {skill.description}");
                    Console.WriteLine($"  效果数量: {skill.effects.Count}");
                    
                    foreach (var effect in skill.effects)
                    {
                        Console.WriteLine($"    - {effect.effectType}: 基础值{effect.baseValue}, 目标{effect.targetType}");
                        if (effect.probability < 1.0f)
                        {
                            Console.WriteLine($"      触发概率: {effect.probability * 100}%");
                        }
                        if (effect.duration > 0)
                        {
                            Console.WriteLine($"      持续时间: {effect.duration}回合");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 模拟技能使用
        /// </summary>
        private static void SimulateSkillUsage(BattleCalculator calculator, 
            BattleUnit playerUnit, BattleUnit enemyUnit)
        {
            Console.WriteLine("\n--- 技能使用模拟 ---");
            
            var skillEngine = calculator.GetSkillEngine();
            var allUnits = new List<BattleUnit> { playerUnit, enemyUnit };
            
            // 模拟玩家使用火球术
            Console.WriteLine("\n1. 玩家使用火球术攻击敌人");
            Console.WriteLine($"攻击前 - 玩家HP: {playerUnit.currentHp}, 敌人HP: {enemyUnit.currentHp}");
            
            var targets = new List<BattleUnit> { enemyUnit };
            var skillResult = skillEngine.ExecuteSkill(playerUnit, 1002, targets);
            
            if (skillResult.success)
            {
                Console.WriteLine($"技能执行成功，产生 {skillResult.actions.Count} 个动作");
                foreach (var action in skillResult.actions)
                {
                    Console.WriteLine($"  动作: {action.description}, 数值: {action.actualValue}");
                }
            }
            
            Console.WriteLine($"攻击后 - 玩家HP: {playerUnit.currentHp}, 敌人HP: {enemyUnit.currentHp}");
            
            // 显示敌人的Buff状态
            var enemyBuffs = skillEngine.GetBuffManager().GetUnitBuffs(enemyUnit.unitId);
            if (enemyBuffs.Count > 0)
            {
                Console.WriteLine($"敌人获得 {enemyBuffs.Count} 个Buff/Debuff:");
                foreach (var buff in enemyBuffs)
                {
                    Console.WriteLine($"  - {buff.buffType}: 持续{buff.remainingDuration}回合, 叠加{buff.currentStacks}层");
                }
            }
            
            // 模拟玩家使用狂暴
            Console.WriteLine("\n2. 玩家使用狂暴技能");
            var selfTargets = new List<BattleUnit> { playerUnit };
            var rageResult = skillEngine.ExecuteSkill(playerUnit, 1004, selfTargets);
            
            if (rageResult.success)
            {
                Console.WriteLine("狂暴技能使用成功");
                var playerBuffs = skillEngine.GetBuffManager().GetUnitBuffs(playerUnit.unitId);
                foreach (var buff in playerBuffs)
                {
                    Console.WriteLine($"  获得增益: {buff.buffType}, 持续{buff.remainingDuration}回合");
                }
            }
        }
        
        /// <summary>
        /// 演示Buff系统
        /// </summary>
        private static void DemonstrateBuffSystem(SkillExecutionEngine skillEngine)
        {
            Console.WriteLine("\n--- Buff系统演示 ---");
            
            var buffManager = skillEngine.GetBuffManager();
            var testUnit = CreateTestUnit(99, "测试单位");
            
            // 创建一个毒伤害效果
            var poisonEffect = new SkillEffect
            {
                effectType = EffectType.AddBuff,
                duration = 3,
                canStack = true,
                maxStacks = 5
            };
            poisonEffect.parameters["tickDamage"] = 5;
            poisonEffect.parameters["specialType"] = "poison";
            
            // 添加毒Buff
            Console.WriteLine("1. 添加毒伤害Buff");
            var poisonBuff = buffManager.AddBuff(testUnit.unitId, poisonEffect, 1, 1002);
            Console.WriteLine($"添加毒Buff: 持续{poisonBuff.remainingDuration}回合, {poisonBuff.currentStacks}层");
            
            // 再次添加（叠加）
            var poisonBuff2 = buffManager.AddBuff(testUnit.unitId, poisonEffect, 1, 1002);
            Console.WriteLine($"叠加毒Buff: 持续{poisonBuff2.remainingDuration}回合, {poisonBuff2.currentStacks}层");
            
            // 模拟回合更新
            Console.WriteLine("\n2. 回合更新演示");
            for (int round = 1; round <= 4; round++)
            {
                Console.WriteLine($"第{round}回合:");
                
                // 更新Buff
                var expiredBuffs = buffManager.UpdateBuffs(testUnit.unitId, round);
                
                var currentBuffs = buffManager.GetUnitBuffs(testUnit.unitId);
                if (currentBuffs.Count > 0)
                {
                    foreach (var buff in currentBuffs)
                    {
                        Console.WriteLine($"  毒Buff: 剩余{buff.remainingDuration}回合, {buff.currentStacks}层");
                        
                        // 模拟毒伤害
                        if (buff.effect.parameters.ContainsKey("tickDamage"))
                        {
                            int tickDamage = Convert.ToInt32(buff.effect.parameters["tickDamage"]) * buff.currentStacks;
                            testUnit.currentHp = Math.Max(0, testUnit.currentHp - tickDamage);
                            Console.WriteLine($"    造成毒伤害: {tickDamage}, 剩余HP: {testUnit.currentHp}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("  无活跃Buff");
                }
                
                if (expiredBuffs.Count > 0)
                {
                    Console.WriteLine($"  {expiredBuffs.Count}个Buff过期");
                }
            }
        }
        
        /// <summary>
        /// 演示复合技能
        /// </summary>
        private static void DemonstrateComplexSkill()
        {
            Console.WriteLine("\n--- 复合技能演示 ---");
            
            // 创建一个复合技能：连击攻击
            var comboSkill = new SkillDefinition
            {
                skillId = 9999,
                skillName = "三连击",
                skillType = SkillType.Active,
                cost = 35,
                cooldown = 4,
                description = "连续攻击目标3次，每次伤害递减",
                effects = new List<SkillEffect>
                {
                    // 第一击
                    new SkillEffect
                    {
                        effectId = 1,
                        effectType = EffectType.Damage,
                        targetType = TargetType.SingleEnemy,
                        baseValue = 20,
                        scaleAttribute = "attack",
                        scaleValue = 1.0f,
                    },
                    // 第二击
                    new SkillEffect
                    {
                        effectId = 2,
                        effectType = EffectType.Damage,
                        targetType = TargetType.SingleEnemy,
                        baseValue = 15,
                        scaleAttribute = "attack",
                        scaleValue = 0.8f,
                    },
                    // 第三击 + 减防Debuff
                    new SkillEffect
                    {
                        effectId = 3,
                        effectType = EffectType.Damage,
                        targetType = TargetType.SingleEnemy,
                        baseValue = 10,
                        scaleAttribute = "attack",
                        scaleValue = 0.6f,
                    },
                    new SkillEffect
                    {
                        effectId = 4,
                        effectType = EffectType.AddBuff,
                        targetType = TargetType.SingleEnemy,
                        duration = 2,
                        probability = 0.9f
                    }
                }
            };
            
            // 添加减防参数
            comboSkill.effects[3].parameters["defenseReduction"] = 8f;
            
            Console.WriteLine($"复合技能: {comboSkill.skillName}");
            Console.WriteLine($"描述: {comboSkill.description}");
            Console.WriteLine($"消耗: {comboSkill.cost}, 冷却: {comboSkill.cooldown}回合");
            Console.WriteLine($"效果数量: {comboSkill.effects.Count}");
            
            for (int i = 0; i < comboSkill.effects.Count; i++)
            {
                var effect = comboSkill.effects[i];
                Console.WriteLine($"  效果{i + 1}: {effect.effectType}");
                Console.WriteLine($"    基础值: {effect.baseValue}, 缩放: {effect.scaleValue}");
                
                if (effect.probability < 1.0f)
                {
                    Console.WriteLine($"    触发概率: {effect.probability * 100}%");
                }
                
                if (effect.duration > 0)
                {
                    Console.WriteLine($"    持续时间: {effect.duration}回合");
                }
            }
            
            // 演示技能模板使用
            Console.WriteLine("\n--- 技能模板演示 ---");
            var skillConfig = SkillConfigManager.Instance;
            
            var templates = new string[] { "BasicAttack", "AOEAttack", "Heal", "Buff", "DamageDebuff" };
            foreach (var templateName in templates)
            {
                var template = skillConfig.GetSkillTemplate(templateName);
                if (template != null)
                {
                    Console.WriteLine($"模板: {template.templateName}");
                    Console.WriteLine($"  类型: {template.skillType}");
                    Console.WriteLine($"  效果模板数量: {template.effectTemplates.Count}");
                    
                    foreach (var effectTemplate in template.effectTemplates)
                    {
                        Console.WriteLine($"    - {effectTemplate.effectType} -> {effectTemplate.targetType}");
                        Console.WriteLine($"      默认参数: {effectTemplate.defaultParameters.Count}个");
                    }
                }
            }
        }
    }
}
