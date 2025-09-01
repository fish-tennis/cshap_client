using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Gserver;

namespace cshap_client.game
{
    // 通用的不可叠加的物品容器(如装备背包或限时道具背包)
    // 不同的实现类,配置不同的解析函数,CfgId,UniqueId的函数即可
    public class UniqueContainer<T> : IElemContainer
    {
        public Func<Any,T> m_ElemCtor; // 自定义构造函数,从protobuf的Any类型构造出T类型的对象
        public Func<T,int> m_CfgIdGetter; // 获取配置id的函数
        public Func<T,long> m_UniqueIdGetter; // 获取unique id的函数
        
        public Dictionary<long, T> m_Elems; // 物品
        public Gserver.ContainerType m_ContainerType; // 容器类型
        public int m_Capacity; // 容量上限
        
        public UniqueContainer(Gserver.ContainerType containerType, int capacity,
            Func<Any,T> elemCtor, Func<T,int> cfgIdGetter, Func<T,long> uniqueIdGetter)
        {
            m_ElemCtor  = elemCtor;
            m_CfgIdGetter = cfgIdGetter;
            m_UniqueIdGetter = uniqueIdGetter;
            m_ContainerType = containerType;
            m_Elems = new Dictionary<long, T>();
            m_Capacity = capacity;
        }
        
        public int GetCapacity()
        {
            return m_Capacity;
        }

        // 获取指定配置id的元素数量
        public int GetElemCount(int elemCfgId)
        {
            return m_Elems.Count(elem => m_CfgIdGetter.Invoke(elem.Value) == elemCfgId);
        }

        // 从proto重置数据
        public void ResetData(MapField<long,T> elems)
        {
            m_Elems.Clear();
            foreach (var elem in elems)
            {
                m_Elems[elem.Key] = elem.Value;
            }
        }
        
        // 响应服务器的数据同步消息(增加,更新,删除)
        public void OnElemSync(Gserver.ElemOp elemOp)
        {
            // 利用protobuf的Any类型,统一不同结构的同步数据,通过自定义构造函数解析出实际的对象
            var elem = m_ElemCtor.Invoke(elemOp.ElemData);
            switch (elemOp.OpType)
            {
                case Gserver.ElemOpType.Add:
                    AddElem(elem);
                    break;
                case Gserver.ElemOpType.Delete:
                    DelElem(m_UniqueIdGetter.Invoke(elem));
                    break;
                case Gserver.ElemOpType.Update:
                    m_Elems[m_CfgIdGetter.Invoke(elem)] = elem;
                    break;
                default:
                    break;
            }
        }

        public void AddElem(T elem)
        {
            m_Elems[m_UniqueIdGetter.Invoke(elem)] = elem;
        }

        public void DelElem(long uniqueId)
        {
            m_Elems.Remove(uniqueId);
        }
    }
}