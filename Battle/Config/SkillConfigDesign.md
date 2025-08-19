# 技能配置设计方案

## 1. 总体设计思路

### 设计原则
- **配置与逻辑分离**：策划配置数据，服务器实现执行逻辑
- **JSON驱动**：使用JSON配置文件，方便策划编辑和版本管理
- **热更新支持**：支持运行时更新配置，无需重启服务器
- **回合制设计**：所有持续时间以回合数为单位
- **6v6战斗**：最多12个单位，快速计算和记录

### 配置文件结构
```
Config/
├── Skills/
│   ├── skill_basic.json      # 基础攻击技能
│   ├── skill_special.json    # 特殊技能
│   ├── skill_buff.json       # 增益技能
│   └── skill_debuff.json     # 减益技能
├── Cards/
│   └── card_definitions.json # 卡牌技能配置
└── Effects/
    └── effect_templates.json # 效果模板
```

## 2. 技能配置格式

### 基础技能配置示例
```json
{
  "skillId": 1001,
  "skillName": "普通攻击",
  "description": "对单个敌人造成物理伤害",
  "skillType": "Active",
  "cost": {
    "type": "Mp",
    "value": 0
  },
  "cooldown": 0,
  "priority": 1,
  "targetType": "SingleEnemy",
  "conditions": [],
  "effects": [
    {
      "effectType": "Damage",
      "probability": 1.0,
      "baseValue": 20,
      "scaleType": "Attack",
      "scaleValue": 1.0,
      "canCrit": true,
      "element": "Physical"
    }
  ]
}
```

### 复合技能配置示例
```json
{
  "skillId": 1002,
  "skillName": "火球术",
  "description": "造成火焰伤害，有概率造成燃烧效果",
  "skillType": "Active",
  "cost": {
    "type": "Mp",
    "value": 20
  },
  "cooldown": 2,
  "priority": 3,
  "targetType": "SingleEnemy",
  "conditions": [],
  "effects": [
    {
      "effectType": "Damage",
      "probability": 1.0,
      "baseValue": 35,
      "scaleType": "MagicAttack",
      "scaleValue": 1.2,
      "canCrit": true,
      "element": "Fire"
    },
    {
      "effectType": "Debuff",
      "probability": 0.8,
      "buffId": "burn",
      "duration": 3,
      "stackType": "Refresh",
      "maxStacks": 5,
      "tickDamage": 8,
      "attributeModifiers": {}
    }
  ]
}
```

## 3. Buff配置设计

### Buff配置示例
```json
{
  "buffId": "burn",
  "buffName": "燃烧",
  "description": "每回合受到火焰伤害",
  "buffType": "Debuff",
  "isVisible": true,
  "icon": "burn_icon",
  "maxStacks": 5,
  "defaultDuration": 3,
  "stackType": "Refresh",
  "dispelType": "Debuff",
  "immunityTags": ["fire_immune"],
  "tickEffect": {
    "effectType": "Damage",
    "baseValue": 8,
    "scaleType": "MagicAttack",
    "scaleValue": 0.2,
    "element": "Fire"
  },
  "attributeModifiers": {},
  "onApply": [],
  "onRemove": [],
  "onTick": []
}
```

## 4. 卡牌技能配置

### 卡牌配置示例
```json
{
  "cardId": 10001,
  "cardName": "火法师",
  "skills": [
    {
      "skillId": 1001,
      "unlockLevel": 1,
      "isDefault": true
    },
    {
      "skillId": 1002,
      "unlockLevel": 1,
      "isDefault": false
    },
    {
      "skillId": 1003,
      "unlockLevel": 5,
      "isDefault": false
    }
  ],
  "aiPreferences": {
    "healThreshold": 0.3,
    "buffProbability": 0.25,
    "aoeThreshold": 2
  }
}
```

## 5. 配置验证规则

### 必填字段验证
- skillId: 必须 > 0 且唯一
- skillName: 不能为空
- effects: 至少包含一个效果
- probability: 必须在 0-1 之间
- duration: 必须 >= 0

### 逻辑验证
- 消耗值不能超过单位最大值
- 冷却时间合理范围 (0-10回合)
- 效果组合的合理性检查
- 引用的buffId必须存在

## 6. 策划配置工具建议

### Excel转JSON工具
```
策划在Excel中配置 → 工具转换为JSON → 服务器加载
```

### 配置模板
- 提供常用技能模板
- 效果组合预设
- 数值范围指导

### 实时预览
- 配置后即时看到效果
- 伤害计算器
- 战斗模拟器
