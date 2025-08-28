using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.game
{
    // 实体
    public interface IEntity
    {
        int GetId();

        IComponent AddComponent(IComponent component);

        IComponent GetComponentByName(string componentName);
    }

    // 实体的组件
    public interface IComponent
    {
        string GetName();

        IEntity GetEntity();

        void SetEntity(IEntity entity);
    }

    public class BaseEntity : IEntity
    {
        // 实体id
        protected int m_Id;
        // 组件列表 TODO:也可以再建一个map,用来快速查询
        protected List<IComponent> m_Components;

        public BaseEntity(int id)
        {
            m_Id = id;
            m_Components = new List<IComponent>();
        }

        public int GetId() { return m_Id; }

        public IComponent GetComponentByName(string componentName)
        {
            foreach (var component in m_Components)
            {
                if (component.GetName() == componentName)
                {
                    return component;
                }
            }
            return null;
        }

        public IComponent AddComponent(IComponent component)
        {
            var old = GetComponentByName(component.GetName());
            if (old != null)
            {
                return old;
            }
            m_Components.Add(component);
            return component;
        }

    }

    public class BaseComponent : IComponent
    {
        private string m_Name;
        private IEntity m_Entity;

        public BaseComponent(string name, IEntity entity)
        {
            m_Name = name;
            m_Entity = entity;
        }

        public IEntity GetEntity()
        {
            return m_Entity;
        }

        public string GetName()
        {
            return m_Name;
        }

        public void SetEntity(IEntity entity)
        {
            m_Entity = entity;
        }
    }

    /// <summary>
    ///  玩家组件的基类
    /// </summary>
    public class BasePlayerComponent : BaseComponent
    {
        public BasePlayerComponent(string name, Player player) : base(name, player)
        {
        }

        public Player GetPlayer()
        {
            return base.GetEntity() as Player;
        }
    }

}
