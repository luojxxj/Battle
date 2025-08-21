using System;
using System.Collections.Generic;
using Server.Battle.Skill;

namespace Server.Battle.Data
{
    /// <summary>
    /// 服务器战斗数据定义
    /// </summary>
    public class ServerBattleData
    {
        #region 基础枚举
        
        /// <summary>
        /// 战斗结果
        /// </summary>
        public enum BattleResult
        {
            Victory = 1,    // 胜利
            Defeat = 2,     // 失败
            Draw = 3        // 平局
        }
        
        #endregion

        #region 数据结构

        /// <summary>
        /// 战斗单位数据
        /// </summary>
        [Serializable]
        public class BattleUnit
        {
            public long unitId;              // 单位ID
            public string unitName;         // 单位名称
            public int AttackId; //普通攻击Id
            public int SkillId;  //主动技能Id
            public List<int> PassiveSkillIds; //被动技能Id列表
            public int maxHp;              // 最大血量
            public int currentHp;          // 当前血量
            public int attack;             // 攻击力
            public int defense;            // 防御力
            public int speed;              // 速度
            public int currentMp;         // 当前怒气值
            public float hpRate;         // 生命百分比加成
            public float attackRate;       // 攻击百分比加成
            public float defenseRate;      // 防御百分比加成
            public float speedRate;        // 速度百分比加成
            public float mpRate;         // 怒气回复百分比加成
            public float hitRate = 1f;         // 命中率加成
            public float dodgeRate;      // 闪避率加成
            public float retaliateRate = 0.1f;     // 反击率加成
            public float resistRetaliateRate; // 抵抗反击率加成
            public float comboRate = 0.1f;        // 连击率加成
            public float resistComboRate;   // 抵抗连击率加成
            public float finalDamageRate;   // 最终伤害加成
            public float finalReduceRate;   // 最终伤害减免加成
            public float finalSkillDamageRate; // 最终技能伤害加成
            public float finalSkillReduceRate; // 最终技能伤害减免加成
            public float criticalRate; // 暴击率
            public float resistCriticalRate; // 抵抗暴击率
            public float criticalDamageRate = 1.5f; // 暴击伤害倍率
            public float resistCriticalDamageRate; // 抵抗暴击伤害倍率
            public int controlCountEnhance; // 控制次数增强
            public int controlCountReduce; // 控制次数减免
            public float vipDamageRate; // VIP伤害加成
            public float vipReduceRate; // VIP伤害减免
            public float healedRate; // 受疗加成
            public float healRate; // 治疗加成
            public float bloodSuckRate; // 吸血倍率
            public float blockRate; // 格挡率
            public float pierceRate; // 穿透率
            public bool isAlive;           // 是否存活
            
            // 统计数据
            public int totalDamageDealt;   // 总造成伤害
            public int totalDamageReceived; // 总承受伤害
            public int totalHealingDone;   // 总治疗量
            public int totalHealingReceived; // 总承受治疗
            
            public BattleUnit()
            {
                isAlive = true;
                totalDamageDealt = 0;
                totalDamageReceived = 0;
                totalHealingDone = 0;
                totalHealingReceived = 0;
            }
            
            /// <summary>
            /// 血量百分比
            /// </summary>
            public float HpPercent => maxHp > 0 ? (float)currentHp / maxHp : 0f;
            
            /// <summary>
            /// 是否濒死
            /// </summary>
            public bool IsNearDeath => HpPercent <= 0.2f;
        }
        
        /// <summary>
        /// 回合数据
        /// </summary>
        [Serializable]
        public class BattleRound
        {
            public int roundNumber;                    // 回合数
            public List<BattleAction> actions;        // 本回合的所有动作
            public Dictionary<long, BattleUnit> unitStates; // 回合结束时的单位状态
            public string roundDescription;           // 回合描述
            
            public BattleRound()
            {
                actions = new List<BattleAction>();
                unitStates = new Dictionary<long, BattleUnit>();
            }
        }
        
        /// <summary>
        /// 完整战斗数据
        /// </summary>
        [Serializable]
        public class CompleteBattleData
        {
            public string battleId;                    // 战斗ID
            public BattleResult result;               // 战斗结果
            public List<BattleRound> rounds;          // 所有回合数据
            public BattleStatistics statistics;       // 战斗统计
            public DateTime battleStartTime;          // 战斗开始时间
            public DateTime battleEndTime;            // 战斗结束时间
            public int totalRounds;                   // 总回合数
            
            public CompleteBattleData()
            {
                rounds = new List<BattleRound>();
                battleId = Guid.NewGuid().ToString();
                battleStartTime = DateTime.Now;
            }
            
            /// <summary>
            /// 战斗持续时间（秒）
            /// </summary>
            public double BattleDurationSeconds => (battleEndTime - battleStartTime).TotalSeconds;
        }

        /// <summary>
        /// 单位统计数据
        /// </summary>
        [Serializable]
        public class UnitStatistics
        {
            public int unitId;                    // 单位ID
            public int damageDealt;              // 造成伤害
            public int damageReceived;           // 承受伤害
            public int healingDone;              // 治疗量
            public int healingReceived;          // 承受治疗
            public int cardsUsed;                // 使用卡牌数
            public int criticalHits;             // 暴击次数
            public int actionCount;              // 行动次数
            public bool survived;                // 是否存活
        }
        
        /// <summary>
        /// 卡牌统计数据
        /// </summary>
        [Serializable]
        public class CardStatistics
        {
            public int cardId;                   // 卡牌ID
            public int timesUsed;               // 使用次数
            public int totalDamage;             // 总伤害
            public int totalHealing;            // 总治疗
            public int criticalHits;            // 暴击次数
            public float averageDamage;         // 平均伤害
            public float effectiveRate;         // 有效率
        }

        /// <summary>
        /// 回合记录
        /// </summary>
        [Serializable]
        public class RoundRecord
        {
            public int roundNumber;                       // 回合号
            public List<BattleAction> actions;            // 本回合的所有动作
            public List<UnitStateSnapshot> unitStatesAtStart; // 回合开始时单位状态
            public List<UnitStateSnapshot> unitStatesAtEnd;   // 回合结束时单位状态
        }

        /// <summary>
        /// 单位状态快照
        /// </summary>
        [Serializable]
        public class UnitStateSnapshot
        {
            public int unitId;
            public int currentHp;
            public int currentMp;
            public int position;
            public bool isAlive;
            public List<BuffState> activeBuffs;
        }

        /// <summary>
        /// 战斗动作记录
        /// </summary>
        [Serializable]
        public class BattleAction
        {
            public ActionType actionType;                 // 动作类型
            public long sourceUnitId;                      // 动作发起者
            public List<long> targetUnitIds;               // 目标单位列表
            public int skillId;                           // 使用的技能ID
            public int roundNumber;                       // 发生的回合
            public bool isCritical;                       // 是否暴击
            public bool isMiss;                           // 是否miss
            public int value;                             // 数值（伤害、治疗等）
            public int actualValue;                      // 实际数值（伤害、治疗等）
            public Dictionary<string, object> actionData; // 动作详细数据
            public string description;                // 动作描述

            public BattleAction()
            {
                targetUnitIds = new List<long>();
                actionData = new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// 战斗统计信息
        /// </summary>
        [Serializable]
        public class BattleStatistics
        {
            public int totalRounds;
            public int totalActions;
            public float battleDuration;
            public int team1UnitsRemaining;
            public int team2UnitsRemaining;
            public int winner;
        }

        public enum ActionType
        {
            Attack,       // 攻击
            UseSkill,     // 使用技能
            BuffApplied,  // 应用Buff
            BuffRemoved,  // 移除Buff
            Heal,         // 治疗
            Death,        // 死亡
            Revive,       // 复活
            Damage,        // 伤害
            Buff,        // Buff
            Debuff,      // Debuff
            StatusEffect, // 状态效果
        }

        #endregion

        #region 配置数据

        /// <summary>
        /// 战斗配置
        /// </summary>
        public static class BattleConfig
        {
            public const int MAX_ROUNDS = 30;           // 最大回合数
            public const int MAX_UNITS_PER_SIDE = 6;    // 每边最大单位数
            public const float CRITICAL_RATE = 0.1f;    // 基础暴击率
            public const float CRITICAL_DAMAGE = 1.5f;  // 暴击伤害倍数
            public const float MISS_RATE = 0.05f;       // 基础miss率
        }
        
        #endregion
    }
}
