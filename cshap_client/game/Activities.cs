using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.game
{
    // 活动的集合组件
    public class Activities : BasePlayerComponent
    {
        public const string ComponentName = "Activities";

        public Dictionary<int, Activity> m_Activities; // 所有活动

        public Activities(Player player) : base(ComponentName, player)
        {
            m_Activities = new Dictionary<int, Activity>();
        }
        
        public Activity GetActivity(int activityId)
        {
            return m_Activities.TryGetValue(activityId, out var activity) ? activity : null;
        }

        // 活动数据同步
        private void OnActivitySync(Gserver.ActivitySync res)
        {
            Console.WriteLine("OnActivitySync:" + res);
            Activity activity = null;
            if (m_Activities.TryGetValue(res.ActivityId, out activity))
            {
                activity.m_Data = res.BaseData;
            }
            else
            {
                activity = new Activity(res.ActivityId, res.BaseData);
                m_Activities[res.ActivityId] = activity;
            }
        }
    }
}
