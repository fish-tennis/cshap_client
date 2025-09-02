using System;
using System.Collections.Generic;
using cshap_client.cfg;
using Gserver;

namespace cshap_client.game
{
    // 背包模块
    // 演示整合多个不同的子背包模块,提供更高一级的背包接口
    // NOTE: 即使游戏里不叫"背包",但是本质还是IElemContainer的功能,依然可以放在背包模块里
    public class Bags : BasePlayerComponent
    {
        public const string ComponentName = "Bags";

        // 普通物品背包
        public CountContainer BagCountItem { get; private set; }
        // 不可叠加的普通物品背包(如限时道具)
        public UniqueItemBag BagUniqueItem { get; private set; }
        // 装备背包
        public EquipBag BagEquip { get; private set; }

        public Bags(Player player) : base(ComponentName, player)
        {
            BagCountItem = new CountContainer(Gserver.ContainerType.CountItem, 0);
            BagUniqueItem = new UniqueItemBag();
            BagEquip = new EquipBag();
        }

        // 获取具体的子背包
        public IElemContainer GetBagByType(Gserver.ContainerType containerType)
        {
            switch (containerType)
            {
                case Gserver.ContainerType.CountItem:
                    return BagCountItem;
                case Gserver.ContainerType.UniqueItem:
                    return BagUniqueItem;
                case Gserver.ContainerType.Equip:
                    return BagEquip;
                default:
                    return null;
            }
        }

        // 根据物品配置id获取对应的子背包
        public IElemContainer GetBagByItemCfgId(int itemCfgId)
        {
            return DataMgr.ItemCfgs.TryGetValue(itemCfgId, out var itemCfg) ? GetBagByItemCfg(itemCfg) : null;
        }

        // 根据物品配置获取对应的子背包
        public IElemContainer GetBagByItemCfg(Gserver.ItemCfg itemCfg)
        {
            switch (itemCfg.ItemType)
            {
                case (int)Gserver.ItemType.None: // 普通物品
                    if (itemCfg.TimeType > 0)
                    {
                        return BagUniqueItem; // 普通物品,但是是限时道具
                    }
                    return BagCountItem;
                case (int)Gserver.ItemType.Equip:
                    return BagEquip;
                default:
                    return null;
            }
            return null;
        }

        // 检查物品是否足够
        public bool IsEnough(IEnumerable<Gserver.ElemNum> items)
        {
            // items可能有重复的物品,所以转换成map来统计总数量
            var itemNumMap = ConvertToItemNumMap(items);
            return IsEnough(itemNumMap);
        }
        
        // 检查物品是否足够
        public bool IsEnough(IEnumerable<Gserver.DelElemArg> items)
        {
            // items可能有重复的物品,所以转换成map来统计总数量
            var itemNumMap = ConvertToItemNumMap(items);
            return IsEnough(itemNumMap);
        }

        // 检查物品是否足够
        public bool IsEnough(Dictionary<int, long> itemNumMap)
        {
            foreach (var kvp in itemNumMap)
            {
                var bag = GetBagByItemCfgId(kvp.Key);
                if (bag == null)
                {
                    return false;
                }
                if (bag.GetElemCount(kvp.Key) < kvp.Value)
                {
                    return false;
                }
            }
            return true;
        }
        
        // items可能有重复的物品,所以转换成map来统计总数量
        public static Dictionary<int, long> ConvertToItemNumMap(IEnumerable<Gserver.ElemNum> items)
        {
            var itemNumMap = new Dictionary<int, long>();
            foreach (var itemNum in items)
            {
                itemNumMap[itemNum.CfgId] = itemNum.Num;
            }
            return itemNumMap;
        }
        
        // items可能有重复的物品,所以转换成map来统计总数量
        public static Dictionary<int, long> ConvertToItemNumMap(IEnumerable<Gserver.DelElemArg> items)
        {
            var itemNumMap = new Dictionary<int, long>();
            foreach (var itemNum in items)
            {
                itemNumMap[itemNum.CfgId] = itemNum.Num;
            }
            return itemNumMap;
        }

        // 同步数据
        private void OnBagsSync(Gserver.BagsSync res)
        {
            Console.WriteLine("OnBagsSync:" + res);
            // 同步普通物品背包
            BagCountItem.ResetData(res.CountItem);
            // 同步不可叠加的普通物品背包
            BagUniqueItem.ResetData(res.UniqueItem);
            // 同步装备背包
            BagEquip.ResetData(res.Equip);
        }

        // 通用的同步子背包的更新数据(增加,更新,删除)
        private void OnElemContainerUpdate(Gserver.ElemContainerUpdate res)
        {
            Console.WriteLine("OnElemContainerUpdate:" + res);
            foreach (var elemOp in res.ElemOps)
            {
                var bag = GetBagByType(elemOp.ContainerType);
                bag?.OnElemSync(elemOp);
            }
        }
        
    }
}