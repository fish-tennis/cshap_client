using Google.Protobuf.WellKnownTypes;

namespace cshap_client.game
{
    // 不可叠加的普通物品背包(如限时道具)
    // Gserver.UniqueCountItem的背包的实现类
    // 只需要实现3个接口即可实现具体protobuf.Message的容器
    public class UniqueItemBag : UniqueContainer<Gserver.UniqueCountItem>
    {
        public UniqueItemBag() : base(Gserver.ContainerType.UniqueItem, 0,
            (Any any) => any.Unpack<Gserver.UniqueCountItem>(),
            (Gserver.UniqueCountItem item) => item.CfgId,
            (Gserver.UniqueCountItem item) => item.UniqueId)
        {
        }
    }
}