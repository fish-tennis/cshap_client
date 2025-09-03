using Gserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cshap_client.cfg;

namespace cshap_client.game
{
    // 兑换组件(如礼包,商店等)
    public class Exchange : BasePlayerComponent
    {
        public const string ComponentName = "Exchange";

        public Dictionary<int, Gserver.ExchangeRecord> Records; // 兑换记录
        
        public Exchange(Player player) : base(ComponentName, player)
        {
            Records = new Dictionary<int, Gserver.ExchangeRecord>();
        }

        // 同步完整数据
        private void OnExchangeSync(Gserver.ExchangeSync res)
        {
            Console.WriteLine("OnExchangeSync:" + res);
            Records.Clear();
            foreach(var kvp in res.Records)
            {
                Records[kvp.Key] = kvp.Value;
            }
        }

        // 更新
        private void OnExchangeUpdate(Gserver.ExchangeUpdate res)
        {
            Console.WriteLine("OnExchangeUpdate:" + res);
            foreach (var v in res.Records)
            {
                Records[v.CfgId] = v;
            }
        }

        // 删除
        private void OnExchangeRemove(Gserver.ExchangeRemove res)
        {
            Console.WriteLine("OnExchangeRemove:" + res);
            foreach (var id in res.CfgIds)
            {
                Records.Remove(id);
            }
        }

        // 兑换回复
        private void OnExchangeRes(Gserver.ExchangeRes res)
        {
            Console.WriteLine("OnExchangeRes:" + res);
            for (int i = 0; i < res.Records.Count; i++)
            {
                var record = res.Records[i];
                Records[record.CfgId] = record;
            }
        }

        // 向服务器发送兑换礼包的请求
        public void ExchangeReq(int cfgId, int count)
        {
            var req = new Gserver.ExchangeReq();
            req.IdCounts.Add(new Gserver.IdCount
            {
                Id = cfgId,
                Count = count,
            });
            Send(req);
        }

        // 批量兑换请求
        public void ExchangeReqBatch(IEnumerable<Gserver.IdCount> idCounts)
        {
            var req = new Gserver.ExchangeReq();
            req.IdCounts.AddRange(idCounts);
            Send(req);
        }
        
        // 检查是否兑换
        public bool CanExchange(int exchangeCfgId, int exchangeCount)
        {
            if (exchangeCount <= 0)
            {
                return false;
            }
            if (!DataMgr.ExchangeCfgs.TryGetValue(exchangeCfgId, out var exchangeCfg))
            {
                return false;
            }
            // 检查配置的条件
            object obj;
            var activityId = ActivityCfgHelper.GetActivityIdByExchangeId(exchangeCfgId);
            if (activityId > 0)
            {
                // 如果是活动礼包,则需要先找到该活动的对象
                obj = GetPlayer().GetActivities().GetActivity(activityId);
            }
            else
            {
                obj = GetPlayer();
            }
            if (obj == null)
            {
                return true;
            }
            if (!Condition.CheckConditions(obj, exchangeCfg.Conditions))
            {
                return false;
            }
            var recordCount = 0;
            if (Records.TryGetValue(exchangeCfgId, out var record))
            {
                recordCount = record.Count;
            }
            return exchangeCount + recordCount >= exchangeCfg.CountLimit; // 检查兑换次数限制
        }
    }
}
