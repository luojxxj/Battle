# 策划技能配置指南

## 📋 配置文件说明

### 技能配置文件结构
```
Config/
├── Skills/                    # 技能配置目录
│   ├── skill_basic.json      # 基础技能（普攻、基础法术）
│   ├── skill_special.json    # 特殊技能（高级技能）
│   ├── skill_buff.json       # 增益技能
│   └── skill_debuff.json     # 减益技能
├── Cards/                     # 卡牌配置目录
│   └── card_definitions.json # 卡牌技能关联
└── Effects/                   # 效果模板目录
    └── effect_templates.json  # 常用效果模板
```

## 🎯 基础技能配置

### 配置字段说明

#### 基础信息
- **skillId**: 技能唯一ID（必填，大于0）
- **skillName**: 技能名称（必填）
- **description**: 技能描述
- **skillType**: 技能类型
  - `Active`: 主动技能（大部分技能）
  - `Passive`: 被动技能
  - `Trigger`: 触发技能
  - `Counter`: 反击技能

#### 消耗和冷却
- **cost.type**: 消耗类型
  - `None`: 无消耗
  - `Mp`: 法力消耗
  - `Hp`: 生命消耗
  - `Energy`: 能量消耗
- **cost.value**: 消耗数值
- **cooldown**: 冷却回合数（0-10建议）
- **priority**: AI使用优先级（1-10，数字越大优先级越高）

#### 目标选择
- **targetType**: 目标类型
  - `Self`: 自己
  - `SingleEnemy`: 单个敌人
  - `SingleAlly`: 单个队友
  - `AllEnemies`: 所有敌人
  - `AllAllies`: 所有队友
  - `LowestHpEnemy`: 血量最低敌人
  - `HighestHpEnemy`: 血量最高敌人
- **maxTargets**: 最大目标数量（AOE技能用）

## ⚡ 技能效果配置

### 效果类型详解

#### 1. 伤害效果 (Damage)
```json
{
  "effectType": "Damage",
  "probability": 1.0,         // 触发概率 (0-1)
  "baseValue": 25,            // 基础伤害
  "scaleType": "Attack",      // 缩放属性
  "scaleValue": 1.2,          // 缩放系数
  "canCrit": true,            // 是否可暴击
  "element": "Fire",          // 元素类型
  "ignoreDefense": false      // 是否无视防御
}
```

**伤害计算公式**:
```
最终伤害 = (基础伤害 + 缩放属性 × 缩放系数) × 暴击倍率 - 防御减免
```

#### 2. 治疗效果 (Heal)
```json
{
  "effectType": "Heal",
  "probability": 1.0,
  "baseValue": 30,            // 基础治疗量
  "scaleType": "MagicAttack", // 缩放属性
  "scaleValue": 0.8,          // 缩放系数
  "canCrit": false,           // 治疗通常不暴击
  "element": "Light"
}
```

#### 3. Buff/Debuff效果
```json
{
  "effectType": "Debuff",
  "probability": 0.8,         // 80%概率触发
  "buffId": "burn",           // Buff ID
  "duration": 3,              // 持续3回合
  "stackType": "Refresh",     // 叠加类型
  "maxStacks": 5,             // 最大5层
  "tickDamage": 8,            // 每回合伤害
  "attributeModifiers": []    // 属性修正
}
```

**叠加类型说明**:
- `None`: 不叠加，新的覆盖旧的
- `Stack`: 可叠加层数和时间
- `Refresh`: 刷新时间，不叠加层数
- `Independent`: 独立存在

## 🎲 释放条件配置

### 常用条件示例

#### 血量条件
```json
{
  "conditionType": "SelfHp",
  "comparison": "Less",       // 小于
  "value": 0.3,              // 30%血量
  "targetTag": ""
}
```

#### 敌人数量条件
```json
{
  "conditionType": "EnemyCount",
  "comparison": "GreaterEqual", // 大于等于
  "value": 2,                   // 2个敌人
  "targetTag": ""
}
```

#### Buff条件
```json
{
  "conditionType": "HasBuff",
  "comparison": "Equal",
  "value": 0,                 // 没有该Buff
  "targetTag": "strength_buff"
}
```

## 🤖 AI配置说明

### AI使用条件
```json
"aiConfig": {
  "selfHpThreshold": 0.5,     // 自身血量阈值
  "enemyHpThreshold": 0.7,    // 敌人血量阈值
  "minEnemyCount": 1,         // 最少敌人数量
  "minAllyCount": 0,          // 最少队友数量
  "useProbability": 0.8,      // 基础使用概率
  "priorityModifier": 0.2,    // 优先级修正
  "targetPreference": "LowestHp" // 目标偏好
}
```

### 目标偏好类型
- `Random`: 随机选择
- `LowestHp`: 优先血量最低
- `HighestHp`: 优先血量最高
- `HighestThreat`: 优先威胁最高
- `Weakest`: 优先最弱
- `Strongest`: 优先最强

## 📝 技能配置模板

### 1. 基础攻击模板
```json
{
  "skillId": 1001,
  "skillName": "普通攻击",
  "skillType": "Active",
  "cost": {"type": "Mp", "value": 0},
  "cooldown": 0,
  "priority": 1,
  "targetType": "SingleEnemy",
  "effects": [{
    "effectType": "Damage",
    "probability": 1.0,
    "baseValue": 20,
    "scaleType": "Attack",
    "scaleValue": 1.0,
    "canCrit": true,
    "element": "Physical"
  }]
}
```

### 2. AOE攻击模板
```json
{
  "skillId": 1003,
  "skillName": "群体攻击",
  "skillType": "Active",
  "cost": {"type": "Mp", "value": 40},
  "cooldown": 4,
  "priority": 5,
  "targetType": "AllEnemies",
  "conditions": [{
    "conditionType": "EnemyCount",
    "comparison": "GreaterEqual",
    "value": 2
  }],
  "effects": [{
    "effectType": "Damage",
    "probability": 1.0,
    "baseValue": 25,
    "scaleType": "MagicAttack",
    "scaleValue": 0.8,
    "canCrit": true,
    "element": "Fire"
  }]
}
```

### 3. 治疗技能模板
```json
{
  "skillId": 1004,
  "skillName": "治疗术",
  "skillType": "Active",
  "cost": {"type": "Mp", "value": 25},
  "cooldown": 1,
  "priority": 2,
  "targetType": "SingleAlly",
  "effects": [{
    "effectType": "Heal",
    "probability": 1.0,
    "baseValue": 30,
    "scaleType": "MagicAttack",
    "scaleValue": 0.8,
    "element": "Light"
  }]
}
```

### 4. Buff技能模板
```json
{
  "skillId": 1005,
  "skillName": "力量祝福",
  "skillType": "Active",
  "cost": {"type": "Mp", "value": 15},
  "cooldown": 3,
  "priority": 2,
  "targetType": "SingleAlly",
  "effects": [{
    "effectType": "Buff",
    "probability": 1.0,
    "buffId": "strength_buff",
    "duration": 5,
    "stackType": "Refresh",
    "attributeModifiers": [{
      "attributeType": "Attack",
      "modifierType": "Add",
      "value": 15,
      "isPercent": false
    }]
  }]
}
```

## ⚠️ 配置注意事项

### 数值平衡建议
1. **伤害数值**: 基础攻击20-30，高级技能30-50
2. **治疗数值**: 治疗量应略低于同等级伤害
3. **法力消耗**: 普攻0MP，技能10-50MP
4. **冷却时间**: 强力技能3-5回合，普通技能1-2回合
5. **持续时间**: Buff/Debuff通常2-5回合

### 常见错误避免
1. **概率设置**: 必须在0-1之间
2. **持续时间**: 不能为负数
3. **技能ID**: 必须唯一且大于0
4. **引用检查**: 确保buffId等引用存在
5. **逻辑合理**: 条件和效果要逻辑一致

### 测试建议
1. 配置后先用单个技能测试
2. 检查AI是否正确使用技能
3. 验证数值平衡性
4. 测试边界情况（0血量、满血等）

## 🔧 配置工具使用

### Excel转JSON工具
1. 在Excel中按模板填写技能数据
2. 使用转换工具生成JSON文件
3. 放入对应配置目录
4. 服务器自动热更新

### 实时调试
1. 修改JSON文件后5秒内自动重载
2. 可在游戏中立即测试效果
3. 错误配置会在控制台显示警告
