using System.Collections.Generic;

namespace cshap_client.game
{
    using ActivityPropertyGetter = System.Func<Activity, string, int>;
    
    public static class ActivityProperty
    {
        // 活动属性值接口 提供一个统一的属性值查询接口
        // 可以在这边把不同模块的值整合到统一的接口里
        // 由Activity.GetPropertyInt32调用
        public static Dictionary<string,ActivityPropertyGetter> Getters = new Dictionary<string, ActivityPropertyGetter>
        {
            {"DayCount",(activity,_)=> activity.GetDayCount()}, // 当前是参加这个活动的第几天,从1开始
        };
    }
}