using Battle.Enum;
using Newtonsoft.Json;

namespace Server.Battle.Config
{
    /// <summary>
    /// 技能配置管理器 - 负责加载和管理策划配置的技能数据
    /// 支持热更新，运行时重新加载配置
    /// 纯服务器端实现，不依赖Unity
    /// </summary>
    public class SkillConfigLoader
    {
        private static SkillConfigLoader _instance;
        private static readonly object _lock = new object();
        public static SkillConfigLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SkillConfigLoader();
                        }
                    }
                }
                return _instance;
            }
        }

        // 配置路径和设置
        public string configRootPath = "Config";
        public bool enableHotReload = true;
        public int hotReloadIntervalMs = 5000; // 5秒检查一次

        // 配置数据存储
        private Dictionary<int, SkillConfigData> _skillConfigs;
        private Dictionary<string, BuffConfigData> _buffConfigs;
        
        // 配置文件监控
        private Dictionary<string, DateTime> _configFileTimestamps;
        private Timer _hotReloadTimer;
        private bool _isInitialized;

        public event Action<int> OnSkillConfigChanged;
        public event Action OnConfigReloaded;

        private SkillConfigLoader()
        {
            InitializeConfig();
        }

        /// <summary>
        /// 初始化配置系统
        /// </summary>
        private void InitializeConfig()
        {
            if (_isInitialized) return;

            _skillConfigs = new Dictionary<int, SkillConfigData>();
            _buffConfigs = new Dictionary<string, BuffConfigData>();
            _configFileTimestamps = new Dictionary<string, DateTime>();

            LoadAllConfigs();

            // 启动热更新定时器
            if (enableHotReload)
            {
                _hotReloadTimer = new Timer(CheckConfigChangesCallback, null, 
                    hotReloadIntervalMs, hotReloadIntervalMs);
            }

            _isInitialized = true;
        }

        /// <summary>
        /// 定时器回调，检查配置变化
        /// </summary>
        private void CheckConfigChangesCallback(object state)
        {
            try
            {
                CheckConfigChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SkillConfigLoader] 热更新检查失败: {e.Message}");
            }
        }

        /// <summary>
        /// 加载所有配置文件
        /// </summary>
        public void LoadAllConfigs()
        {
            try
            {
                LoadSkillConfigs();
                LoadBuffConfigs();
                
                ValidateConfigs();
                
                Console.WriteLine($"[SkillConfigLoader] 配置加载完成: {_skillConfigs.Count}个技能");
                OnConfigReloaded?.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SkillConfigLoader] 配置加载失败: {e.Message}");
            }
        }

        /// <summary>
        /// 加载技能配置
        /// </summary>
        private void LoadSkillConfigs()
        {
            _skillConfigs.Clear();
            
            // 获取当前程序目录下的配置路径
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string skillConfigPath = Path.Combine(baseDir, configRootPath, "Skills");
            
            if (!Directory.Exists(skillConfigPath))
            {
                Console.WriteLine($"[SkillConfigLoader] 技能配置目录不存在: {skillConfigPath}");
                return;
            }

            string[] jsonFiles = Directory.GetFiles(skillConfigPath, "*.json");
            
            foreach (string filePath in jsonFiles)
            {
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    SkillConfigData[] skills = JsonConvert.DeserializeObject<SkillConfigData[]>(jsonContent);
                    
                    foreach (var skill in skills)
                    {
                        if (_skillConfigs.ContainsKey(skill.skillId))
                        {
                            Console.WriteLine($"[SkillConfigLoader] 重复的技能ID: {skill.skillId}");
                            continue;
                        }
                        
                        _skillConfigs[skill.skillId] = skill;
                    }
                    
                    // 记录文件时间戳
                    _configFileTimestamps[filePath] = File.GetLastWriteTime(filePath);
                    
                    Console.WriteLine($"[SkillConfigLoader] 加载技能配置: {Path.GetFileName(filePath)} ({skills.Length}个技能)");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[SkillConfigLoader] 加载技能配置失败 {filePath}: {e.Message}");
                }
            }
        }

        /// <summary>
        /// 加载Buff配置
        /// </summary>
        private void LoadBuffConfigs()
        {
            _buffConfigs.Clear();

            // 获取当前程序目录下的Buff配置路径
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string buffConfigPath = Path.Combine(baseDir, configRootPath, "Buffs");

            if (!Directory.Exists(buffConfigPath))
            {
                Console.WriteLine($"[SkillConfigLoader] Buff配置目录不存在: {buffConfigPath}");
                return;
            }

            string[] jsonFiles = Directory.GetFiles(buffConfigPath, "*.json");

            foreach (string filePath in jsonFiles)
            {
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    BuffConfigData[] buffs = JsonConvert.DeserializeObject<BuffConfigData[]>(jsonContent);

                    foreach (var buff in buffs)
                    {
                        if (string.IsNullOrEmpty(buff.buffId))
                        {
                            Console.WriteLine($"[SkillConfigLoader] Buff配置缺少buffId: {buff.buffName}");
                            continue;
                        }
                        if (_buffConfigs.ContainsKey(buff.buffId))
                        {
                            Console.WriteLine($"[SkillConfigLoader] 重复的BuffID: {buff.buffId}");
                            continue;
                        }
                        _buffConfigs[buff.buffId] = buff;
                    }

                    // 记录文件时间戳
                    _configFileTimestamps[filePath] = File.GetLastWriteTime(filePath);

                    Console.WriteLine($"[SkillConfigLoader] 加载Buff配置: {Path.GetFileName(filePath)} ({buffs.Length}个Buff)");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[SkillConfigLoader] 加载Buff配置失败 {filePath}: {e.Message}");
                }
            }
        }


        /// <summary>
        /// 验证配置数据的完整性和正确性
        /// </summary>
        private void ValidateConfigs()
        {
            int errorCount = 0;
            
            // 验证技能配置
            foreach (var kvp in _skillConfigs)
            {
                if (!ValidateSkillConfig(kvp.Value))
                {
                    errorCount++;
                    Console.WriteLine($"[SkillConfigLoader] 技能配置验证失败: {kvp.Key} - {kvp.Value.skillId}");
                }
            }
            
            if (errorCount > 0)
            {
                Console.WriteLine($"[SkillConfigLoader] 配置验证完成，发现 {errorCount} 个错误");
            }
        }

        /// <summary>
        /// 验证单个技能配置
        /// </summary>
        private bool ValidateSkillConfig(SkillConfigData skill)
        {
            // 基础验证
            if (skill.skillId <= 0)
            {
                Console.WriteLine($"技能ID必须大于0: {skill.skillId}");
                return false;
            }
            
            if (skill.effects == null || skill.effects.Count == 0)
            {
                Console.WriteLine($"技能必须至少有一个效果: {skill.skillId}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查配置文件是否有变化
        /// </summary>
        private void CheckConfigChanges()
        {
            bool hasChanges = false;
            
            foreach (var kvp in _configFileTimestamps)
            {
                string filePath = kvp.Key;
                DateTime lastTimestamp = kvp.Value;
                
                if (File.Exists(filePath))
                {
                    DateTime currentTimestamp = File.GetLastWriteTime(filePath);
                    if (currentTimestamp > lastTimestamp)
                    {
                        hasChanges = true;
                        Console.WriteLine($"[SkillConfigLoader] 检测到配置文件变化: {Path.GetFileName(filePath)}");
                        break;
                    }
                }
            }
            
            if (hasChanges)
            {
                Console.WriteLine("[SkillConfigLoader] 重新加载配置...");
                LoadAllConfigs();
            }
        }

        // 公共API方法
        
        /// <summary>
        /// 获取技能配置
        /// </summary>
        public SkillConfigData GetSkillConfig(int skillId)
        {
            return _skillConfigs.TryGetValue(skillId, out SkillConfigData config) ? config : null;
        }

        /// <summary>
        /// 获取所有技能配置
        /// </summary>
        public Dictionary<int, SkillConfigData> GetAllSkillConfigs()
        {
            return new Dictionary<int, SkillConfigData>(_skillConfigs);
        }

        /// <summary>
        /// 手动重新加载配置
        /// </summary>
        public void ManualReload()
        {
            LoadAllConfigs();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _hotReloadTimer?.Dispose();
            _hotReloadTimer = null;
        }
    }

    /// <summary>
    /// 卡牌配置数据
    /// </summary>
    [Serializable]
    public class CardConfigData
    {
        public int cardId;
        public string cardName;
        public string description;
        public string rarity;
        public List<CardSkillRef> skills;
        public AIPreferences aiPreferences;
    }

    /// <summary>
    /// 卡牌技能引用
    /// </summary>
    [Serializable]
    public class CardSkillRef
    {
        public int skillId;
        public int unlockLevel;
        public bool isDefault;
    }

    /// <summary>
    /// AI偏好设置
    /// </summary>
    [Serializable]
    public class AIPreferences
    {
        public float healThreshold;      // 治疗阈值
        public float buffProbability;    // 使用增益技能概率
        public int aoeThreshold;         // AOE技能使用的敌人数量阈值
        public float aggressiveness;     // 攻击性 (0-1)
    }

    /// <summary>
    /// Buff配置数据
    /// </summary>
    [Serializable]
    public class BuffConfigData
    {
        public string buffId;
        public string buffName;
        public string description;
        public EffectType buffType;
        public bool isVisible;
        public string icon;
        public int maxStacks;
        public int defaultDuration;
        public StackType stackType;
        public string dispelType;
        public string[] immunityTags;
        public SkillEffectData tickEffect;
        public AttributeModifierData[] attributeModifiers;
    }
}
