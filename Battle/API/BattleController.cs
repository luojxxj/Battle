using Server.Battle.Logic;
using static Server.Battle.Data.ServerBattleData;

namespace Server.Battle.API
{
    /// <summary>
    /// 战斗API控制器
    /// 提供HTTP API接口处理客户端战斗请求
    /// </summary>
    public class BattleController
    {
        #region 私有字段
        
        private readonly ServerBattleManager _battleManager;
        
        #endregion

        #region 构造函数
        
        public BattleController()
        {
            _battleManager = ServerBattleManager.Instance;
        }
        
        #endregion

        #region PVE战斗接口
        
        /// <summary>
        /// 开始战斗
        /// POST /api/battle/pve/start
        /// </summary>
        /// <param name="request">PVE战斗请求</param>
        /// <returns>战斗响应</returns>
        public async Task<BattleLoadCompleteResponse> StartBattle(StartBattleRequest request)
        {
            try
            {
                // 参数验证
                if (request == null)
                {
                    return new BattleLoadCompleteResponse
                    {
                        success = false,
                        message = "请求参数不能为空"
                    };
                }
                
                Console.WriteLine($"[BattleController] 收到战斗请求 - 玩家: {request.playerId}");
                
                // 调用战斗管理器处理
                var response = await _battleManager.HandleBattleRequest(
                    request.playerId,
                    request.TeamOne,
                    request.TeamTwo);
                
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BattleController] PVE战斗接口异常: {ex.Message}");
                
                return new BattleLoadCompleteResponse
                {
                    success = false,
                    message = "服务器内部错误"
                };
            }
        }
        
        #endregion

        #region 查询接口
        
        /// <summary>
        /// 获取战斗数据
        /// GET /api/battle/data/{battleId}
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <param name="battleId">战斗ID</param>
        /// <returns>战斗数据</returns>
        public CompleteBattleData GetBattleData(int playerId, string battleId)
        {
            try
            {
                Console.WriteLine($"[BattleController] 查询战斗数据 - 玩家: {playerId}, 战斗ID: {battleId}");
                
                return _battleManager.GetBattleData(playerId, battleId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BattleController] 查询战斗数据异常: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 获取玩家战斗历史
        /// GET /api/battle/history/{playerId}
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>战斗历史</returns>
        public async Task<BattleHistoryResponse> GetBattleHistory(int playerId, int pageIndex = 1, int pageSize = 20)
        {
            try
            {
                Console.WriteLine($"[BattleController] 查询战斗历史 - 玩家: {playerId}");
                
                // TODO: 实现从数据库查询战斗历史
                await Task.Delay(50);
                
                return new BattleHistoryResponse
                {
                    success = true,
                    totalCount = 0,
                    battles = new System.Collections.Generic.List<BattleHistoryItem>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BattleController] 查询战斗历史异常: {ex.Message}");
                
                return new BattleHistoryResponse
                {
                    success = false,
                    message = "查询失败"
                };
            }
        }
        
        #endregion

        #region 管理接口（仅供管理员使用）
        
        /// <summary>
        /// 获取服务器战斗统计
        /// GET /api/battle/admin/stats
        /// </summary>
        /// <returns>服务器统计</returns>
        public async Task<ServerBattleStats> GetServerBattleStats()
        {
            try
            {
                // TODO: 实现服务器战斗统计查询
                await Task.Delay(100);
                
                return new ServerBattleStats
                {
                    totalBattlesCount = 1000,
                    activeBattlesCount = 10,
                    averageBattleTime = 45.5,
                    winRate = 0.65f
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BattleController] 查询服务器统计异常: {ex.Message}");
                return null;
            }
        }
        
        #endregion
    }

    #region 请求数据结构
    
    public class Hero
    {
        public long Uid;     //英雄唯一Id
        public Dictionary<int, float> AttrDic;  //英雄属性字典，key为属性Id，value为属性值
        public int AttackId; //英雄普通攻击Id
        public int SkillId;  //英雄主动技能Id
        public List<int> PassiveSkillIds; //英雄被动技能Id列表
    }

    /// <summary>
    /// PVE战斗开始请求
    /// </summary>
    [Serializable]
    public class StartBattleRequest
    {
        public int playerId;               // 玩家ID
        public List<Hero> TeamOne;         //队伍一
        public List<Hero> TeamTwo;         // 队伍二
    }
    
    /// <summary>
    /// 战斗结束请求
    /// </summary>
    [Serializable]
    public class BattleEndRequest
    {
        public int playerId;               // 玩家ID
        public string battleId;           // 战斗ID
        public bool playbackCompleted;    // 是否完整播放
        public double playbackTime;       // 播放时间（秒）
        public bool skipped;              // 是否跳过
    }
    
    #endregion

    #region 响应数据结构
    
    /// <summary>
    /// 战斗历史响应
    /// </summary>
    [Serializable]
    public class BattleHistoryResponse
    {
        public bool success;
        public string message;
        public int totalCount;
        public System.Collections.Generic.List<BattleHistoryItem> battles;
    }
    
    /// <summary>
    /// 战斗历史项
    /// </summary>
    [Serializable]
    public class BattleHistoryItem
    {
        public string battleId;            // 战斗ID
        public DateTime battleTime;        // 战斗时间
        public BattleResult result;        // 战斗结果
        public int dungeonId;             // 副本ID（PVE）
        public int opponentId;            // 对手ID（PVP）
        public int totalRounds;           // 总回合数
        public double battleDuration;     // 战斗持续时间
    }
    
    /// <summary>
    /// 服务器战斗统计
    /// </summary>
    [Serializable]
    public class ServerBattleStats
    {
        public int totalBattlesCount;      // 总战斗次数
        public int activeBattlesCount;     // 活跃战斗数
        public double averageBattleTime;   // 平均战斗时间
        public float winRate;              // 胜率
        public DateTime lastUpdateTime;    // 最后更新时间
    }
    
    #endregion
}
