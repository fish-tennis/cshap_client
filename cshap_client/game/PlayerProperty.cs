using System.Collections.Generic;

namespace cshap_client.game
{
    using PlayerPropertyGetter = System.Func<Player, string, Gserver.ConditionCfg, int>;
    
    public static class PlayerProperty
    {
        // 玩家属性值接口 提供一个统一的属性值查询接口
        // 可以在这边把不同模块的值整合到统一的接口里
        // 由Player.GetPropertyInt32调用
        public static Dictionary<string,PlayerPropertyGetter> Getters = new Dictionary<string, PlayerPropertyGetter>
        {
            {"Level",(player,propertyName,conditionCfg)=> player.BaseInfo.data.Level}, // 等级
            {"TotalPay",(player,propertyName,conditionCfg)=> player.BaseInfo.data.TotalPay}, // 总支付金额
            {"FinishQuestCount",(player,propertyName,conditionCfg)=> player.GetQuest().Finished.Count}, // 完成任务数量
            {"PassStage",GetPassStage}, // 是否通关 演示代码
        };

        // 通关的属性,演示代码
        public static int GetPassStage(Player player, string propertyName, Gserver.ConditionCfg conditionCfg)
        {
            if (conditionCfg.Options.Count == 0)
            {
                return 0;
            }
            // 参数1:关卡Id 参数2:通关星数
            // 只填1个参数,表示只检查通关,填了参数2,表示还要检查通关星数
            if (conditionCfg.Options.Count == 1)
            {
                var stageId = conditionCfg.Options[0];
                // TODO: 其他模块的数据来检查通关情况
                return 0;
            }
            if (conditionCfg.Options.Count == 2)
            {
                var stageId = conditionCfg.Options[0];
                var passStar = conditionCfg.Options[1];
                // TODO: 其他模块的数据来检查通关情况
                return 0;
            }
            return 0;
        }
    }
}