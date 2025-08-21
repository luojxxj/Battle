using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Battle.API;
using Server.Battle.Data;
using Server.Battle.Example;
using Server.Battle.Logic;
using static Server.Battle.Data.ServerBattleData;

namespace Server.Battle
{
    /// <summary>
    /// 战斗服务器示例
    /// 演示如何使用战斗系统处理客户端请求
    /// </summary>
    public class BattleServerExample
    {
        private BattleController _battleController;

        public BattleServerExample()
        {
            _battleController = new BattleController();
        }

        /// <summary>
        /// 启动服务器示例
        /// </summary>
        public async Task StartServer()
        {
            Console.WriteLine("=== 战斗服务器启动 ===");
            Console.WriteLine("等待客户端请求...");

            //模拟技能释放
            SkillSystemExample.RunExample();

            // 模拟处理一些战斗请求
            await SimulateBattleRequests();

            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        /// <summary>
        /// 模拟战斗请求处理
        /// </summary>
        private async Task SimulateBattleRequests()
        {
            // 模拟战斗请求
            Console.WriteLine("\n--- 模拟PVE战斗 ---");
            await SimulateBattle();

            await Task.Delay(2000);

            // 模拟查询请求
            Console.WriteLine("\n--- 模拟数据查询 ---");
            await SimulateDataQueries();
        }

        /// <summary>
        /// 模拟战斗
        /// </summary>
        private async Task SimulateBattle()
        {
            var request = new StartBattleRequest
            {
                playerId = 12345,
            };

            request.TeamOne = new List<Hero>();
            request.TeamTwo = new List<Hero>();
            // 添加一些模拟英雄到队伍
            request.TeamOne.Add(new Hero
            {
                Uid = 1,
                AttackId = 101, // 普通攻击ID
                SkillId = 201, // 主动技能ID
                PassiveSkillIds = new List<int> { 301, 302 }, // 被动技能ID列表
                AttrDic = new Dictionary<int, float> { { 1001, 100 }, { 1002, 10}, { 1003 , 20 }, { 1004 , 121 } }
            });

            request.TeamTwo.Add(new Hero
            {
                Uid = 2,
                AttackId = 102, // 普通攻击ID
                SkillId = 202, // 主动技能ID
                PassiveSkillIds = new List<int> { 303, 304 }, // 被动技能ID列表
                AttrDic = new Dictionary<int, float> { { 1001, 100 }, { 1002, 8 }, { 1003, 21 }, { 1004, 125 } }
            });

            // 开始战斗
            var battleResponse = await _battleController.StartBattle(request);
            
            if (battleResponse.success)
            {
                Console.WriteLine($"战斗开始成功！战斗ID: {battleResponse.battleId}");
                Console.WriteLine($"战斗结果: {battleResponse.battleData.result}");
                Console.WriteLine($"总回合数: {battleResponse.battleData.totalRounds}");
                Console.WriteLine($"战斗时长: {battleResponse.battleData.BattleDurationSeconds:F2}秒");

                // 打印回合详情
                PrintBattleDetails(battleResponse.battleData);
            }
            else
            {
                Console.WriteLine($"战斗开始失败: {battleResponse.message}");
            }
        }

        /// <summary>
        /// 模拟数据查询
        /// </summary>
        private async Task SimulateDataQueries()
        {
            // 查询战斗历史
            var historyResponse = await _battleController.GetBattleHistory(12345);
            Console.WriteLine($"战斗历史查询: {(historyResponse.success ? "成功" : "失败")}");

            // 查询服务器统计
            var serverStats = await _battleController.GetServerBattleStats();
            if (serverStats != null)
            {
                Console.WriteLine($"服务器统计 - 总战斗数: {serverStats.totalBattlesCount}, 活跃战斗: {serverStats.activeBattlesCount}");
                Console.WriteLine($"平均战斗时间: {serverStats.averageBattleTime:F1}秒, 胜率: {serverStats.winRate:P1}");
            }
        }

        /// <summary>
        /// 打印战斗详情
        /// </summary>
        private void PrintBattleDetails(CompleteBattleData battleData)
        {
            Console.WriteLine("\n=== 战斗回合详情 ===");
            
            foreach (var round in battleData.rounds)
            {
                Console.WriteLine($"\n回合 {round.roundNumber}: {round.roundDescription}");
                
                foreach (var action in round.actions.Take(3)) // 只显示前3个动作
                {
                    var source = round.unitStates[action.sourceUnitId];
                    var target = round.unitStates[action.targetUnitIds.FirstOrDefault()];
                    
                    string actionDesc = $"  - {source.unitName} → {target.unitName}";
                    
                    if (action.actionType == ServerBattleData.ActionType.Attack || action.actionType == ServerBattleData.ActionType.Damage)
                    {
                        actionDesc += $" 造成 {action.actualValue} 点伤害";
                        if (action.isCritical) actionDesc += " (暴击)";
                        if (action.isMiss) actionDesc += " (Miss)";
                    }
                    else if (action.actionType == ServerBattleData.ActionType.Heal)
                    {
                        actionDesc += $" 恢复 {action.actualValue} 点生命";
                    }
                    
                    Console.WriteLine(actionDesc);
                }
                
                if (round.actions.Count > 3)
                {
                    Console.WriteLine($"  ... 还有 {round.actions.Count - 3} 个动作");
                }
            }
        }

        /// <summary>
        /// 主入口点
        /// </summary>
        public static async Task Main(string[] args)
        {
            var server = new BattleServerExample();
            await server.StartServer();
        }
    }
}
