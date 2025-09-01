using Google.Protobuf.WellKnownTypes;

namespace cshap_client.game
{
    // 容器通用操作接口(例如背包就是一种容器)
    public interface IElemContainer
    {
        // 容量
        int GetCapacity();

        // 获取指定配置id的元素数量
        int GetElemCount(int elemCfgId);

        // 响应服务器的数据同步消息(增加,更新,删除)
        void OnElemSync(Gserver.ElemOp elemOp);
    }

    // 有唯一id的对象(如可升级的装备)
    public interface IUniquely
    {
        // 配置id
        int GetCfgId();

        // unique id
        long GetUniqueId();
    }

    // 限时类对象
    public interface ITimeLimited
    {
        // 超时时间戳
        int GetTimeout();
    }
    
    // 配置数据提供一个统一的接口,以方便做一些统一的处理
    public interface ICfgData
    {
        // 配置id
        int GetCfgId();
    }
}