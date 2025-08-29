using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.game
{
    // 玩家对象
    public class Player : BaseEntity
    {
        public string Name { get; set; } // 玩家名
        public int AccountId { get; set; } // 账号id
        public int RegionId { get; set; } // 区服id

        public Player(int id) : base(id)
        {
        }

        // 初始化玩家的所有组件
        public void InitComponents()
        {
            // TODO:可以通过C#的自定义属性来自动添加组件(在组件类上设置自定义属性)
            AddComponent(new BaseInfo(this));
            AddComponent(new Quest(this));
            AddComponent(new Exchange(this));
            AddComponent(new Activities(this));
        }

        // 遍历玩家组件
        public void RangePlayerComponents(Action<BasePlayerComponent> rangeAction)
        {
            foreach (var component in base.m_Components)
            {
                rangeAction.Invoke(component as BasePlayerComponent);
            }
        }

        public BaseInfo GetBaseInfo()
        {
            return GetComponentByName(BaseInfo.ComponentName) as BaseInfo;
        }

        public Quest GetQuest()
        {
            return GetComponentByName(Quest.ComponentName) as Quest;
        }

    }

    
}
