using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Google.Protobuf.Collections;

namespace cshap_client.game
{
    // 有数量的容器(如常见的道具背包,每个格子只需要记录一个配置id和数量即可)
    public class CountContainer : IElemContainer
    {
        public Dictionary<int, int> m_Elems; // 物品
        public Gserver.ContainerType m_ContainerType; // 容器类型
        public int m_Capacity; // 容量上限

        public CountContainer(Gserver.ContainerType containerType, int capacity)
        {
            m_ContainerType = containerType;
            m_Elems = new Dictionary<int, int>();
            m_Capacity = capacity;
        }
        
        // 容量
        public int GetCapacity()
        {
            return m_Capacity;
        }

        // 获取指定配置id的元素数量
        public int GetElemCount(int elemCfgId)
        {
            m_Elems.TryGetValue(elemCfgId, out var elemCount);
            return elemCount;
        }

        // 从proto重置数据
        public void ResetData(MapField<int,int> elems)
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
            // 普通物品用的结构是Gserver.ElemNum(id和数量)
            var elemNum = elemOp.ElemData.Unpack<Gserver.ElemNum>();
            switch (elemOp.OpType)
            {
                case Gserver.ElemOpType.Add:
                    AddElem(elemNum);
                    break;
                case Gserver.ElemOpType.Delete:
                    DelElem(elemNum);
                    break;
                case Gserver.ElemOpType.Update:
                    m_Elems[elemNum.CfgId] = elemNum.Num;
                    break;
                default:
                    break;
            }
        }

        // 添加元素
        public void AddElem(Gserver.ElemNum elemNum)
        {
            m_Elems.TryGetValue(elemNum.CfgId, out var elemCount);
            m_Elems[elemNum.CfgId] = elemCount + elemNum.Num;
        }

        // 删除指定数量元素
        public void DelElem(Gserver.ElemNum elemNum)
        {
            var found = m_Elems.TryGetValue(elemNum.CfgId, out var elemCount);
            if (elemCount <= elemNum.Num)
            {
                m_Elems.Remove(elemNum.CfgId);
            }
            else if (found)
            {
                m_Elems[elemNum.CfgId] = elemCount - elemNum.Num;
            }
        }
    }
}