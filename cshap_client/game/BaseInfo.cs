using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.game
{
    // 玩家的基础信息组件
    public class BaseInfo : BasePlayerComponent
    {
        public const string ComponentName = "BaseInfo";
        public Gserver.BaseInfo data;
        public BaseInfo(Player player) : base(ComponentName, player)
        {
        }

        // 同步数据
        public void OnBaseInfoSync(Gserver.BaseInfoSync res)
        {
            data = res.Data;
            Console.WriteLine("OnBaseInfoSync:" + data);
        }
    }
}
