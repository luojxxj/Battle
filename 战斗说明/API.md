# 战斗服务器 API

## 1. 开始战斗

### 1.1 PVE战斗

**请求:**

*   **URL:** `/api/battle/pve/start`
*   **Method:** `POST`
*   **Body:**

```json
{
  "playerId": 12345,
  "TeamOne": [
    {
      "Uid": 1,
      "AttackId": 101,
      "SkillId": 201,
      "PassiveSkillIds": [301, 302],
      "AttrDic": {
        "1001": 100,
        "1002": 10,
        "1003": 20,
        "1004": 121
      }
    }
  ],
  "TeamTwo": [
    {
      "Uid": 2,
      "AttackId": 102,
      "SkillId": 202,
      "PassiveSkillIds": [303, 304],
      "AttrDic": {
        "1001": 100,
        "1002": 8,
        "1003": 21,
        "1004": 125
      }
    }
  ]
}
```

**响应:**

*   **Status Code:** `200 OK`
*   **Body:**

```json
{
  "success": true,
  "battleId": "b617b09d-d4e4-4ab6-a9c9-2a15ae884c1d",
  "battleData": {
    // 详细战斗数据
  },
  "message": "战斗计算完成"
}
```

### 1.2 PVP战斗

**请求:**

*   **URL:** `/api/battle/pvp/start`
*   **Method:** `POST`
*   **Body:**

```json
{
  "playerId": 12345,
  "TeamOne": [
    {
      "Uid": 1,
      "AttackId": 101,
      "SkillId": 201,
      "PassiveSkillIds": [301, 302],
      "AttrDic": {
        "1001": 100,
        "1002": 10,
        "1003": 20,
        "1004": 121
      }
    }
  ],
  "TeamTwo": [
    {
      "Uid": 2,
      "AttackId": 102,
      "SkillId": 202,
      "PassiveSkillIds": [303, 304],
      "AttrDic": {
        "1001": 100,
        "1002": 8,
        "1003": 21,
        "1004": 125
      }
    }
  ]
}
```

**响应:**

*   **Status Code:** `200 OK`
*   **Body:**

```json
{
  "success": true,
  "battleId": "b617b09d-d4e4-4ab6-a9c9-2a15ae884c1d",
  "battleData": {
    // 详细战斗数据
  },
  "message": "战斗计算完成"
}
```

## 2. 战报数据结构

`CompleteBattleData`

```csharp
public class CompleteBattleData
{
    public string battleId;                    // 战斗ID
    public BattleResult result;               // 战斗结果
    public List<BattleRound> rounds;          // 所有回合数据
    public BattleStatistics statistics;       // 战斗统计
    public DateTime battleStartTime;          // 战斗开始时间
    public DateTime battleEndTime;            // 战斗结束时间
    public int totalRounds;                   // 总回合数
}
```

`BattleRound`

```csharp
public class BattleRound
{
    public int roundNumber;                    // 回合数
    public List<BattleAction> actions;        // 本回合的所有动作
    public Dictionary<long, BattleUnit> unitStates; // 回合结束时的单位状态
    public string roundDescription;           // 回合描述
}
```

`BattleAction`

```csharp
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
}
```

`BattleUnit`

```csharp
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
}
```

`BattleStatistics`

```csharp
public class BattleStatistics
{
    public int totalRounds;
    public int totalActions;
    public float battleDuration;
    public int team1UnitsRemaining;
    public int team2UnitsRemaining;
    public int winner;
}
```