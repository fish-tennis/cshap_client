using System;
using Google.Protobuf.WellKnownTypes;
using Gserver;

namespace cshap_client.game
{
    // 装备背包(这里演示的普通的装备背包,像RPG那种能拖动格子的背包需要另行实现)
    // 只需要实现3个接口即可实现具体protobuf.Message的容器
    public class EquipBag : UniqueContainer<Gserver.Equip>
    {
        public EquipBag() : base(ContainerType.Equip, 0,
            (Any any) => any.Unpack<Equip>(),
            (Equip equip) => equip.CfgId,
            (Equip equip) => equip.UniqueId)
        {
        }
    }
}