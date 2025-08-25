using Server.Battle.Core;
using static Server.Battle.Data.ServerBattleData;
using Server.Battle.API;

namespace Server.Battle.Logic
{
    /// <summary>
    /// 服务器战斗管理器
    /// 负责处理客户端的战斗请求并返回完整的战斗数据
    /// </summary>
    public class ServerBattleManager
    {
        #region 私有字段

        private static ServerBattleManager _instance;
        private static readonly object _lock = new object();

        private Dictionary<string, CompleteBattleData> _activeBattles;
        private BattleCalculator _battleCalculator;

        #endregion

        #region 单例模式

        public static ServerBattleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ServerBattleManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private ServerBattleManager()
        {
            _activeBattles = new Dictionary<string, CompleteBattleData>();
            _battleCalculator = new BattleCalculator();
        }

        #endregion

        #region 公共接口

        /// <summary>
        /// 处理战斗请求
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <param name="teamOne">队伍一</param>
        /// <param name="teamTwo">队伍二</param>
        /// <returns>完整战斗数据</returns>
        public async Task<BattleLoadCompleteResponse> HandleBattleRequest(
            int playerId,
            List<Hero> teamOne,
            List<Hero> teamTwo)
        {
            try
            {
                Console.WriteLine($"[ServerBattleManager] 收到战斗请求 - 玩家: {playerId}");

                // 验证请求参数
                if (!ValidateBattleRequest(playerId, teamOne, teamTwo))
                {
                    return CreateErrorResponse("战斗请求参数无效");
                }

                // 开始异步计算战斗
                var battleData = await Task.Run(() =>
                    _battleCalculator.CalculateBattle(teamOne, teamTwo));

                // 存储战斗数据供后续查询
                _activeBattles[battleData.battleId] = battleData;

                // 记录战斗日志
                await LogBattleStart(playerId, battleData);

                Console.WriteLine($"[ServerBattleManager] PVE战斗计算完成 - 战斗ID: {battleData.battleId}, 结果: {battleData.result}");

                return new BattleLoadCompleteResponse
                {
                    success = true,
                    battleId = battleData.battleId,
                    battleData = battleData,
                    message = "战斗计算完成"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServerBattleManager] PVE战斗处理异常: {ex.Message}");
                return CreateErrorResponse("服务器处理战斗时发生错误");
            }
        }

        /// <summary>
        /// 获取战斗数据（用于查询或重新获取）
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <param name="battleId">战斗ID</param>
        /// <returns>战斗数据</returns>
        public CompleteBattleData GetBattleData(int playerId, string battleId)
        {
            if (!_activeBattles.ContainsKey(battleId))
            {
                return null;
            }

            var battleData = _activeBattles[battleId];


            return battleData;
        }

        #endregion

        #region 私有验证方法

        /// <summary>
        /// 验证战斗请求
        /// </summary>
        private bool ValidateBattleRequest(int playerId, List<Hero> teamOne, List<Hero> teamTwo)
        {
            if (playerId <= 0)
            {
                Console.WriteLine("[ServerBattleManager] 无效的玩家ID");
                return false;
            }

            if (teamOne == null || teamOne.Count == 0 || teamOne.Count > BattleConfig.MAX_UNITS_PER_SIDE)
            {
                Console.WriteLine("[ServerBattleManager] teamOne 单位数量无效");
                return false;
            }

            if (teamTwo == null || teamTwo.Count == 0 || teamTwo.Count > BattleConfig.MAX_UNITS_PER_SIDE)
            {
                Console.WriteLine("[ServerBattleManager] teamTwo 单位数量无效");
                return false;
            }

            return true;
        }

        #endregion

        #region 数据库操作方法（示例）

        /// <summary>
        /// 记录战斗开始日志
        /// </summary>
        private async Task LogBattleStart(int playerId, CompleteBattleData battleData)
        {
            await Task.Delay(10); // 模拟数据库操作

            // TODO: 实现战斗日志记录
            Console.WriteLine($"[ServerBattleManager] 记录战斗开始 - 玩家: {playerId}, 战斗ID: {battleData.battleId}");
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 创建错误响应
        /// </summary>
        private BattleLoadCompleteResponse CreateErrorResponse(string message)
        {
            return new BattleLoadCompleteResponse
            {
                success = false,
                message = message
            };
        }

        #endregion
    }

    #region 响应数据结构

    /// <summary>
    /// 战斗加载完成响应
    /// </summary>
    [Serializable]
    public class BattleLoadCompleteResponse
    {
        public bool success;                        // 是否成功
        public string battleId;                     // 战斗ID
        public CompleteBattleData battleData;       // 完整战斗数据
        public string message;                      // 响应消息
    }

    #endregion
}
