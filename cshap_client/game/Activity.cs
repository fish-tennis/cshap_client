using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cshap_client.cfg;

namespace cshap_client.game
{
    // 单个活动的实现,所有活动使用同一个类来实现
    // 差异化部分通过动态属性和自定义回调来处理
    // 设计思路: 活动本身只是任务和兑换的组合,具体的任务数据和兑换数据都在玩家的Quest组件和Exchange组件里
    public class Activity : IPropertyInt32
    {
        public Activities m_Activities;
        public int Id;
        public Gserver.ActivityCfg m_Cfg; // 配置数据
        public Gserver.ActivityDefaultBaseData m_Data; // 活动数据

        public Activity(int activityId, Gserver.ActivityDefaultBaseData data)
        {
            Id = activityId;
            m_Data = data;
            DataMgr.ActivityCfgs.TryGetValue(Id, out var value);
            m_Cfg = value;
        }

        // 获取活动数据上的int32属性值(先查找m_Data,如果没有再查找该活动配置的Properties)
        // IPropertyInt32的实现
        public int GetPropertyInt32(string property)
        {
            return m_Data.PropertiesInt32.TryGetValue(property, out var value) ? value : GetCfgPropertyInt32(property);
        }

        // 获取活动配置的int32属性值
        public int GetCfgPropertyInt32(string property)
        {
            if(!m_Cfg.Properties.TryGetValue(property, out var value))
            {
                return 0;
            }
            int.TryParse(value, out var i);
            return i;
        }

        // 获取活动配置的string属性值
        public string GetCfgPropertyString(string property)
        {
            m_Cfg.Properties.TryGetValue(property, out var value);
            return value;
        }
    }
}
