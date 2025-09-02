using System.Collections.Generic;

namespace cshap_client.cfg
{
    // 活动相关配置预处理
    public static class ActivityCfgHelper
    {
        // 兑换id和活动id的映射表
        // NOTE: 要求同一个兑换id不能用于不同的活动!!!
        public static Dictionary<int, int> ExchangeIdsByActivity;

        public static void AfterLoad()
        {
            ExchangeIdsByActivity = new Dictionary<int, int>();
            foreach (var activityCfg in DataMgr.ActivityCfgs.Values)
            {
                foreach (var exchangeId in activityCfg.ExchangeIds)
                {
                    ExchangeIdsByActivity[exchangeId] = activityCfg.CfgId;
                }
            }
        }
        
        // 兑换id对应的活动id
        public static int GetActivityIdByExchangeId(int exchangeId)
        {
            return ExchangeIdsByActivity.TryGetValue(exchangeId, out var activityId) ? activityId : 0;
        }
    }
}