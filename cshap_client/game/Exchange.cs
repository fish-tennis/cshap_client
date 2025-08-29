using Gserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Records.Add(kvp.Key, kvp.Value);
            }
        }

        // 更新
        private void OnExchangeUpdate(Gserver.ExchangeUpdate res)
        {
            Console.WriteLine("OnExchangeUpdate:" + res);
            foreach (var v in res.Records)
            {
                Records.Add(v.CfgId, v);
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

        // 向服务器发送兑换礼包的请求(TODO: 改成支持批量兑换)
        public void ExchangeReq(int cfgId, int count)
        {
            Client.Send(new Gserver.ExchangeReq
            {
                CfgId = cfgId,
                Count = count,
            });
        }
    }
}
