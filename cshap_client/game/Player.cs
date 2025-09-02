using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace cshap_client.game
{
    // 玩家对象
    public class Player : BaseEntity, IPropertyInt32
    {
        public string Name { get; set; } // 玩家名
        public int AccountId { get; set; } // 账号id
        public int RegionId { get; set; } // 区服id

        // 组件可以在这里保存一个引用,查找组件的时候,就可以直接获取到
        public BaseInfo BaseInfo { get; private set; }

        public Player(int id) : base(id)
        {
        }

        // 初始化玩家的所有组件
        public void InitComponents()
        {
            // TODO:也可以通过C#的自定义属性来自动添加组件(在组件类上设置自定义属性)
            // 这里先手动写,也没问题
            BaseInfo =  AddComponent(new BaseInfo(this)) as BaseInfo;
            AddComponent(new Quest(this));
            AddComponent(new Exchange(this));
            AddComponent(new Activities(this));
            AddComponent(new Bags(this));
        }

        // 遍历玩家组件
        public void RangePlayerComponents(Action<BasePlayerComponent> rangeAction)
        {
            foreach (var component in base.m_Components)
            {
                rangeAction.Invoke(component as BasePlayerComponent);
            }
        }

        public Quest GetQuest()
        {
            return GetComponentByName(Quest.ComponentName) as Quest;
        }
        
        public Exchange GetExchange()
        {
            return GetComponentByName(Exchange.ComponentName) as Exchange;
        }
        
        public Activities GetActivities()
        {
            return GetComponentByName(Activities.ComponentName) as Activities;
        }
        
        public Bags GetBags()
        {
            return GetComponentByName(Bags.ComponentName) as Bags;
        }
        
        // 获取玩家数据上的int32属性值
        // IPropertyInt32的实现
        public int GetPropertyInt32(string property)
        {
            return PlayerProperty.Getters.TryGetValue(property, out var getter) ? getter(this, property) : 0;
        }
        
        public bool Send(IMessage message)
        {
            return Client.Instance.m_Connection != null && Client.Instance.m_Connection.Send(message);
        }

    }

    
}
