using Battle.Enum;
using Server.Battle.API;
using Server.Battle.Config;
using Server.Battle.Data;
using Server.Battle.Skill;
using static Server.Battle.Data.ServerBattleData;

namespace Server.Battle.Core
{
    /// <summary>
    /// 服务器战斗计算器
    /// 负责快速演算完整的战斗过程
    /// </summary>
    public class BattleCalculator
    {
        #region 私有字段

        private Random _random;
        private Dictionary<int, BattleUnit> _playerUnits; // 站位Id -> 战斗单位
        private Dictionary<int, BattleUnit> _enemyUnits;  // 站位Id -> 战斗单位
        private List<BattleUnit> _turnOrder;
        private CompleteBattleData _battleData;
        private SkillExecutionEngine _skillEngine;
        private SkillConfigManager _skillConfig;
        private Dictionary<long, List<int>> _unitSkills; // 单位ID -> 技能ID列表
        private Dictionary<long, Dictionary<int, int>> _skillCooldowns; // 单位ID -> (技能ID -> 剩余冷却)
        private SkillConfigLoader _skillConfigLoader; //技能加载器

        #endregion

        #region 构造函数

        public BattleCalculator()
        {
            _random = new Random();
            _playerUnits = new Dictionary<int, BattleUnit>();
            _enemyUnits = new Dictionary<int, BattleUnit>();
            _turnOrder = new List<BattleUnit>();
            _skillEngine = new SkillExecutionEngine();
            _skillConfig = SkillConfigManager.Instance;
            _unitSkills = new Dictionary<long, List<int>>();
            _skillCooldowns = new Dictionary<long, Dictionary<int, int>>();
            _skillConfigLoader = SkillConfigLoader.Instance;

            // 注册所有技能定义到执行引擎
            var allSkills = _skillConfig.GetAllSkillDefinitions();
            _skillEngine.RegisterSkillDefinitions(allSkills);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 计算完整的战斗
        /// </summary>
        /// <param name="teamOne">队伍一</param>
        /// <param name="teamTwo">队伍二</param>
        /// <returns>完整战斗数据</returns>
        public CompleteBattleData CalculateBattle(List<Hero> teamOne, List<Hero> teamTwo)
        {
            Console.WriteLine($"[BattleCalculator] 开始计算战斗");

            // 初始化战斗数据
            _battleData = new CompleteBattleData
            {
                battleStartTime = DateTime.Now
            };

            // 初始化单位
            InitializePlayerUnits(teamOne);
            InitializeEnemyUnits(teamTwo);

            // 计算回合顺序
            CalculateTurnOrder();

            // 执行战斗计算
            ExecuteBattle();

            // 完成战斗统计
            _battleData.battleEndTime = DateTime.Now;
            _battleData.totalRounds = _battleData.rounds.Count;
            CalculateBattleStatistics();

            Console.WriteLine($"[BattleCalculator] 战斗计算完成，结果: {_battleData.result}, 回合数: {_battleData.totalRounds}");

            return _battleData;
        }

        /// <summary>
        /// 获取技能执行引擎
        /// </summary>
        public SkillExecutionEngine GetSkillEngine()
        {
            return _skillEngine;
        }

        /// <summary>
        /// 获取单位的技能列表
        /// </summary>
        public List<int> GetUnitSkills(int unitId)
        {
            return _unitSkills.ContainsKey(unitId) ? _unitSkills[unitId] : new List<int>();
        }

        /// <summary>
        /// 获取单位的技能冷却状态
        /// </summary>
        public Dictionary<int, int> GetUnitSkillCooldowns(int unitId)
        {
            return _skillCooldowns.ContainsKey(unitId) ? _skillCooldowns[unitId] : new Dictionary<int, int>();
        }

        #endregion

        #region 初始化方法

        /// <summary>
        /// 初始化玩家单位
        /// </summary>
        private void InitializePlayerUnits(List<Hero> teamHero)
        {
            _playerUnits.Clear();

            for (int i = 0; i < teamHero.Count; i++)
            {
                var unit = CreatePlayerUnit(teamHero[i]);
                _playerUnits[i] = unit;
            }

            Console.WriteLine($"[BattleCalculator] 初始化玩家单位完成，数量: {_playerUnits.Count}");
        }

        /// <summary>
        /// 初始化敌方单位
        /// </summary>
        private void InitializeEnemyUnits(List<Hero> teamHero)
        {
            _enemyUnits.Clear();

            for (int i = 0; i < teamHero.Count; i++)
            {
                var unit = CreatePlayerUnit(teamHero[i]);
                _enemyUnits[i] = unit;
            }

            Console.WriteLine($"[BattleCalculator] 初始化敌方单位完成，数量: {_playerUnits.Count}");
        }

        /// <summary>
        /// 初始化单位技能
        /// </summary>
        public void InitializeUnitSkills(long unitId, List<int> skillIds)
        {
            _unitSkills[unitId] = new List<int>(skillIds);
            _skillCooldowns[unitId] = new Dictionary<int, int>();

            // 初始化技能冷却
            foreach (var skillId in skillIds)
            {
                _skillCooldowns[unitId][skillId] = 0;
            }
        }

        #endregion

        #region 战斗执行

        /// <summary>
        /// 执行战斗
        /// </summary>
        private void ExecuteBattle()
        {
            int roundNumber = 1;

            while (!IsBattleFinished() && roundNumber <= BattleConfig.MAX_ROUNDS)
            {
                Console.WriteLine($"[BattleCalculator] 执行第 {roundNumber} 回合");

                var round = ExecuteRound(roundNumber);
                _battleData.rounds.Add(round);

                roundNumber++;
            }

            // 确定战斗结果
            _battleData.result = DetermineBattleResult();
        }

        /// <summary>
        /// 执行单个回合
        /// </summary>
        private BattleRound ExecuteRound(int roundNumber)
        {
            var round = new BattleRound
            {
                roundNumber = roundNumber
            };

            // 按照速度顺序执行每个单位的行动
            foreach (var unit in _turnOrder)
            {
                if (!unit.isAlive) continue;

                var action = DecideUnitAction(unit, roundNumber);
                if (action != null)
                {
                    ExecuteAction(action);
                    round.actions.Add(action);
                }

                // 检查战斗是否结束
                if (IsBattleFinished())
                    break;
            }

            // 保存回合结束时的单位状态
            SaveRoundEndStates(round);

            // 生成回合描述
            round.roundDescription = GenerateRoundDescription(round);

            return round;
        }

        /// <summary>
        /// 决定单位行动
        /// </summary>
        protected BattleAction DecideUnitAction(BattleUnit unit, int roundNumber)
        {
            // 回合开始判定
            OnRoundStart(roundNumber);

            // 判断单位是否可以行动


            // 判断单位使用技能还是基础攻击

            var selectedSkill = (SkillDefinition)null;

            // 执行技能
            var targets = SelectSkillTargets(unit, selectedSkill);
            var skillResult = _skillEngine.ExecuteSkill(unit, selectedSkill.skillId, targets, GetAllUnits());

            if (skillResult.success && skillResult.actions.Count > 0)
            {
                // 返回第一个动作（主要动作）
                var mainAction = skillResult.actions[0];

                // 处理额外动作（如果有）
                for (int i = 1; i < skillResult.actions.Count; i++)
                {
                    // 可以将额外动作添加到当前回合或下一步骤
                    AddAdditionalAction(skillResult.actions[i]);
                }

                return mainAction;
            }

            //回合结束判定
            OnRoundEnd(roundNumber);

            // 技能执行失败，使用基础攻击
            return null;
        }

        /// <summary>
        /// 执行动作
        /// </summary>
        private void ExecuteAction(BattleAction action)
        {
            var source = GetUnitById(action.sourceUnitId);
            var target = GetUnitByIds(action.targetUnitIds);

            // 执行动作效果
            switch (action.actionType)
            {
                case ActionType.Attack:
                    ExecuteDamageAction(source, target, action);
                    break;

                case ActionType.UseSkill:
                    ExecuteSkillAction(source, target, action);
                    break;
            }

            // 更新统计数据
            UpdateActionStatistics(action);

            // 检查死亡
            CheckUnitDeath(target);
        }   

        #endregion

        #region 动作执行

        /// <summary>
        /// 执行伤害动作
        /// </summary>
        private void ExecuteDamageAction(BattleUnit source, List<BattleUnit> target, BattleAction action)
        {

        }

        /// <summary>
        /// 执行技能动作
        /// </summary>
        private void ExecuteSkillAction(BattleUnit source, List<BattleUnit> target, BattleAction action)
        {

        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 回合开始处理
        /// </summary>
        private void OnRoundStart(int roundNumber)
        {
            // 更新所有单位的Buff
            _skillEngine.UpdateAllBuffs(GetAllUnits(), roundNumber);

            // 处理Buff触发的效果
            ProcessBuffEffects(roundNumber);
        }

        /// <summary>
        /// 回合结束处理
        /// </summary>
        /// <param name="roundNumber"></param>
        private void OnRoundEnd(int roundNumber)
        {

        }

        /// <summary>
        /// 添加额外动作
        /// </summary>
        private void AddAdditionalAction(BattleAction action)
        {
            // 这里可以将额外动作添加到当前回合的动作列表中
            // 具体实现取决于你的战斗流程设计
        }

        /// <summary>
        /// 选择技能目标
        /// </summary>
        private List<BattleUnit> SelectSkillTargets(
            BattleUnit caster,
            SkillDefinition skill)
        {
            var targets = new List<BattleUnit>();
            var allUnits = GetAllUnits();

            // 基于技能的第一个效果选择目标（简化处理）
            if (skill.effects.Count > 0)
            {
                var mainEffect = skill.effects[0];
                targets = SelectTargetsBySelection(caster, mainEffect.targetType, allUnits);
            }

            return targets;
        }

        /// <summary>
        /// 根据目标选择类型选择目标
        /// </summary>
        private List<BattleUnit> SelectTargetsBySelection(
            BattleUnit caster,
            TargetType targetSelection,
            List<BattleUnit> allUnits)
        {
            var targets = new List<BattleUnit>();

            return targets;
        }

        /// <summary>
        /// 获取所有单位（需要在基类中实现或重写）
        /// </summary>
        protected virtual List<BattleUnit> GetAllUnits()
        {
            // 这里需要根据基类的实际实现来获取所有单位
            // 示例实现：
            var allUnits = new List<BattleUnit>();
            // allUnits.AddRange(_playerUnits.Values);
            // allUnits.AddRange(_enemyUnits.Values);
            return allUnits;
        }


        /// <summary>
        /// 计算回合顺序
        /// </summary>
        private void CalculateTurnOrder()
        {
            _turnOrder.Clear();

            // 合并所有单位
            var allUnits = _playerUnits.Values.Concat(_enemyUnits.Values).ToList();

            // 按速度排序
            _turnOrder = allUnits.OrderByDescending(u => u.speed).ThenBy(u => _random.Next()).ToList();

            Console.WriteLine($"[BattleCalculator] 回合顺序计算完成，参战单位: {_turnOrder.Count}");
        }

        /// <summary>
        /// 根据ID获取单位
        /// </summary>
        private BattleUnit GetUnitById(long unitId)
        {
            var allUnits = _playerUnits.Values.Concat(_enemyUnits.Values).ToList();
            return allUnits.Find(m => m.unitId == unitId);
        }

        private List<BattleUnit> GetUnitByIds(List<long> unitIds)
        {
            var result = new List<BattleUnit>();
            // 假设只传入一个ID
            if (unitIds.Count == 0) return null;
            foreach (var unitId in unitIds)
            {
                var unit = GetUnitById(unitId);
                if (unit != null)
                {
                    result.Add(unit);
                }
            }

            return result;
        }

        /// <summary>
        /// 检查单位死亡
        /// </summary>
        private void CheckUnitDeath(List<BattleUnit> units)
        {
            foreach (var unit in units)
            {
                if (unit.currentHp <= 0 && unit.isAlive)
                {
                    unit.isAlive = false;
                    Console.WriteLine($"[BattleCalculator] {unit.unitName} 死亡");
                }
            }
        }

        /// <summary>
        /// 检查战斗是否结束
        /// </summary>
        private bool IsBattleFinished()
        {
            bool playerAlive = _playerUnits.Values.Any(u => u.isAlive);
            bool enemyAlive = _enemyUnits.Values.Any(u => u.isAlive);

            return !playerAlive || !enemyAlive;
        }

        /// <summary>
        /// 确定战斗结果
        /// </summary>
        private BattleResult DetermineBattleResult()
        {
            bool playerAlive = _playerUnits.Values.Any(u => u.isAlive);
            bool enemyAlive = _enemyUnits.Values.Any(u => u.isAlive);

            if (playerAlive && !enemyAlive)
                return BattleResult.Victory;
            else if (!playerAlive && enemyAlive)
                return BattleResult.Defeat;
            else
                return BattleResult.Draw;
        }

        #endregion

        #region 冷却和条件管理

        /// <summary>
        /// 检查技能是否在冷却中
        /// </summary>
        private bool IsSkillOnCooldown(int unitId, int skillId)
        {
            if (!_skillCooldowns.ContainsKey(unitId)) return false;
            if (!_skillCooldowns[unitId].ContainsKey(skillId)) return false;

            return _skillCooldowns[unitId][skillId] > 0;
        }

        /// <summary>
        /// 设置技能冷却
        /// </summary>
        private void SetSkillCooldown(int unitId, int skillId, int cooldown)
        {
            if (!_skillCooldowns.ContainsKey(unitId))
            {
                _skillCooldowns[unitId] = new Dictionary<int, int>();
            }

            _skillCooldowns[unitId][skillId] = cooldown;
        }

        /// <summary>
        /// 更新技能冷却
        /// </summary>
        private void UpdateSkillCooldowns(int unitId)
        {
            if (!_skillCooldowns.ContainsKey(unitId)) return;

            var cooldowns = _skillCooldowns[unitId];
            var keys = cooldowns.Keys.ToList();

            foreach (var skillId in keys)
            {
                if (cooldowns[skillId] > 0)
                {
                    cooldowns[skillId]--;
                }
            }
        }

        /// <summary>
        /// 检查是否可以使用技能
        /// </summary>
        private bool CanUseSkill(BattleUnit unit, SkillDefinition skill)
        {
            // 检查释放条件
            foreach (var condition in skill.conditions)
            {
                if (!EvaluateSkillCondition(unit, condition))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 评估技能条件
        /// </summary>
        private bool EvaluateSkillCondition(BattleUnit unit, SkillCondition condition)
        {
            float actualValue = 0f;
            var allUnits = GetAllUnits();

            switch (condition.conditionType)
            {
                case "SelfHp":
                    actualValue = unit.HpPercent;
                    break;
                case "EnemyCount":
                    actualValue = allUnits.Count(u => u.isAlive);
                    break;
                case "AllyCount":
                    actualValue = allUnits.Count(u => u.isAlive);
                    break;
            }

            return CompareValues(actualValue, condition.comparison, condition.value);
        }

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

        #endregion

        #region Buff处理

        /// <summary>
        /// 处理Buff效果
        /// </summary>
        private void ProcessBuffEffects(int roundNumber)
        {
            var allUnits = GetAllUnits();

            foreach (var unit in allUnits)
            {
                var buffs = _skillEngine.GetBuffManager().GetUnitBuffs(unit.unitId);

                foreach (var buff in buffs)
                {
                    // 处理持续性效果（如毒伤害、生命恢复等）
                    if (buff.ShouldTick(roundNumber))
                    {
                        ProcessBuffTick(unit, buff);
                        buff.tickCounter = roundNumber;
                    }
                }
            }
        }

        /// <summary>
        /// 处理Buff触发
        /// </summary>
        private void ProcessBuffTick(ServerBattleData.BattleUnit unit, BuffState buff)
        {
            var effect = buff.effect;

            if (effect.parameters.ContainsKey("tickDamage"))
            {
                // 持续伤害（如毒、燃烧）
                int tickDamage = Convert.ToInt32(effect.parameters["tickDamage"]) * buff.currentStacks;
                unit.currentHp = Math.Max(0, unit.currentHp - tickDamage);

                Console.WriteLine($"[EnhancedBattleCalculator] {unit.unitName} 受到持续伤害 {tickDamage} 点");

                // 创建伤害动作记录
                AddTickDamageAction(unit, buff, tickDamage);
            }

            if (effect.parameters.ContainsKey("tickHeal"))
            {
                // 持续治疗
                int tickHeal = Convert.ToInt32(effect.parameters["tickHeal"]) * buff.currentStacks;
                int actualHeal = Math.Min(tickHeal, unit.maxHp - unit.currentHp);
                unit.currentHp += actualHeal;

                Console.WriteLine($"[EnhancedBattleCalculator] {unit.unitName} 恢复生命值 {actualHeal} 点");

                // 创建治疗动作记录
                AddTickHealAction(unit, buff, actualHeal);
            }
        }

        /// <summary>
        /// 添加持续伤害动作记录
        /// </summary>
        private void AddTickDamageAction(BattleUnit unit, BuffState buff, int damage)
        {
            var action = new BattleAction
            {
                actionType = ActionType.Damage,
                sourceUnitId = buff.sourceUnitId,
                value = damage,
                actualValue = damage,
                description = $"{unit.unitName} 受到持续伤害"
            };

            // 添加到当前回合动作列表
            AddAdditionalAction(action);
        }

        /// <summary>
        /// 添加持续治疗动作记录
        /// </summary>
        private void AddTickHealAction(BattleUnit unit, BuffState buff, int heal)
        {
            var action = new BattleAction
            {
                actionType = ActionType.Heal,
                sourceUnitId = buff.sourceUnitId,
                targetUnitIds = new List<long> { unit.unitId },
                value = heal,
                actualValue = heal,
                description = $"{unit.unitName} 持续恢复生命值"
            };

            // 添加到当前回合动作列表
            AddAdditionalAction(action);
        }

        #endregion

        #region 数据处理方法

        /// <summary>
        /// 保存回合结束状态
        /// </summary>
        private void SaveRoundEndStates(BattleRound round)
        {
            foreach (var kvp in _playerUnits)
            {
                round.unitStates[kvp.Key] = CloneUnit(kvp.Value);
            }

            foreach (var kvp in _enemyUnits)
            {
                round.unitStates[kvp.Key] = CloneUnit(kvp.Value);
            }
        }

        /// <summary>
        /// 克隆单位
        /// </summary>
        private BattleUnit CloneUnit(BattleUnit original)
        {
            return new BattleUnit
            {
                unitId = original.unitId,
                unitName = original.unitName,
                maxHp = original.maxHp,
                currentHp = original.currentHp,
                attack = original.attack,
                defense = original.defense,
                speed = original.speed,
                isAlive = original.isAlive,
                totalDamageDealt = original.totalDamageDealt,
                totalDamageReceived = original.totalDamageReceived,
                totalHealingDone = original.totalHealingDone,
                totalHealingReceived = original.totalHealingReceived
            };
        }

        #endregion

        #region 配置加载方法（需要实现）

        /// <summary>
        /// 创建战斗单位
        /// </summary>
        private BattleUnit CreatePlayerUnit(Hero hero)
        {
            var unit =  new BattleUnit
            {
                unitId = hero.Uid,
                unitName = $"玩家单位{hero.Uid}",
                AttackId = hero.AttackId,
                SkillId = hero.SkillId,
                PassiveSkillIds = hero.PassiveSkillIds,
            };

            foreach (var attr in hero.AttrDic)
            {
                switch ((AttributeType)attr.Key)
                {
                    case AttributeType.Hp:
                        unit.maxHp = (int)attr.Value;
                        unit.currentHp = unit.maxHp;
                        break;
                    case AttributeType.Attack:
                        unit.attack = (int)attr.Value;
                        break;
                    case AttributeType.Defense:
                        unit.defense = (int)attr.Value;
                        break;
                    case AttributeType.Speed:
                        unit.speed = (int)attr.Value;
                        break;
                    case AttributeType.Mp:
                        unit.currentMp = (int)attr.Value;
                        break;
                    case AttributeType.HpRate:
                        unit.hpRate = attr.Value;
                        break;
                    case AttributeType.AttackRate:
                        unit.attackRate = attr.Value;
                        break;
                    case AttributeType.DefenseRate:
                        unit.defenseRate = attr.Value;
                        break;
                    case AttributeType.SpeedRate:
                        unit.speedRate = attr.Value;
                        break;
                    case AttributeType.MpRate:
                        unit.mpRate = attr.Value;
                        break;
                    case AttributeType.HitRate:
                        unit.hitRate = attr.Value;
                        break;
                    case AttributeType.DodgeRate:
                        unit.dodgeRate = attr.Value;
                        break;
                    case AttributeType.RetaliateRate:
                        unit.retaliateRate = attr.Value;
                        break;
                    case AttributeType.ResistRetaliateRate:
                        unit.resistRetaliateRate = attr.Value;
                        break;
                    case AttributeType.ComboRate:
                        unit.comboRate = attr.Value;
                        break;
                    case AttributeType.ResistComboRate:
                        unit.resistComboRate = attr.Value;
                        break;
                    case AttributeType.FinalDamageRate:
                        unit.finalDamageRate = attr.Value;
                        break;
                    case AttributeType.FinalReduceRate:
                        unit.finalReduceRate = attr.Value;
                        break;
                    case AttributeType.FinalSkillDamageRate:
                        unit.finalSkillDamageRate = attr.Value;
                        break;
                    case AttributeType.FinalSkillReduceRate:
                        unit.finalSkillReduceRate = attr.Value;
                        break;
                    case AttributeType.CriticalRate:
                        unit.criticalRate = attr.Value;
                        break;
                    case AttributeType.ResistCriticalRate:
                        unit.resistCriticalRate = attr.Value;
                        break;
                    case AttributeType.CriticalDamageRate:
                        unit.criticalDamageRate = attr.Value;
                        break;
                    case AttributeType.ResistCriticalDamageRate:
                        unit.resistCriticalDamageRate = attr.Value;
                        break;
                    case AttributeType.ControlCountEnhance:
                        unit.controlCountEnhance = (int)attr.Value;
                        break;
                    case AttributeType.ControlCountReduce:
                        unit.controlCountReduce = (int)attr.Value;
                        break;
                    case AttributeType.VipDamageRate:
                        unit.vipDamageRate = attr.Value;
                        break;
                    case AttributeType.VipReduceRate:
                        unit.vipReduceRate = attr.Value;
                        break;
                    case AttributeType.HealedRate:
                        unit.healedRate = attr.Value;
                        break;
                    case AttributeType.HealRate:
                        unit.healRate = attr.Value;
                        break;
                    case AttributeType.BloodSuckRate:
                        unit.bloodSuckRate = attr.Value;
                        break;
                    case AttributeType.BlockRate:
                        unit.blockRate = attr.Value;
                        break;
                    case AttributeType.PierceRate:
                        unit.pierceRate = attr.Value;
                        break;
                }
            }

            return unit;
        }

        /// <summary>
        /// 生成回合描述
        /// </summary>
        private string GenerateRoundDescription(BattleRound round)
        {
            if (round.actions.Count == 0)
                return $"第{round.roundNumber}回合：无动作";

            return "回合结束";
        }

        /// <summary>
        /// 更新动作统计
        /// </summary>
        private void UpdateActionStatistics(BattleAction action)
        {
        }

        /// <summary>
        /// 计算战斗统计
        /// </summary>
        private void CalculateBattleStatistics()
        {
            _battleData.statistics = new BattleStatistics();
        }

        #endregion
    }
}
