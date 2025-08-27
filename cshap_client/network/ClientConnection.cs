using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using cshap_client.game;
using gnet_csharp;
using Google.Protobuf;

namespace cshap_client.network
{
    internal class ClientConnection : TcpConnection
    {
        // 是否使用网关模式,使用网关模式时,客户端只需要连接网关服务器,由网关服务器转发消息给后端服务器
        // 使用网关模式时,连接登录服务器,可以不使用http
        public bool m_IsUseGateMode = true;
        private long m_LastPingTimestamp; // 上一次ping的时间戳
        public const int PingInterval = 5; // 几秒钟ping一次

        public ClientConnection(ConnectionConfig connectionConfig, int connectionId) : base(connectionConfig, connectionId)
        {
            Tag = this;
            OnConnected = onConnected;
            OnClose = onClose;
        }

        private void onConnected(IConnection connection, bool success)
        {
            Console.WriteLine("onConnected host:" + GetHostAddress() + " success:" + success);
        }

        private void onClose(IConnection connection)
        {
            Console.WriteLine("onClose");
        }

        // 发protobuf消息,会自动查找对应的消息号
        public bool Send(IMessage message)
        {
            ushort cmdId = PacketCommandMapping.GetCommandByProto(message);
            if (cmdId == 0)
            {
                return false;
            }
            return Send(cmdId, message);
        }

        public void AutoPing()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now - m_LastPingTimestamp >= PingInterval)
            {
                m_LastPingTimestamp = now;
                Send(new Gserver.HeartBeatReq
                {
                    Timestamp = now,
                });
                //Console.WriteLine("AutoPing:" + now);
            }
        }

        public void ProcessPackets()
        {
            while (Client.Instance.IsRunning)
            {
                var packet = PopPacket();
                if (packet == null)
                {
                    return;
                }
                var descriptor = PacketCommandMapping.GetMessageDescriptorByCommand(packet.Command());
                if (descriptor != null && descriptor.Name == "HeartBeatRes")
                {
                    continue;
                }
                // 消息回调
                if (packet.Message() == null)
                {
                    Console.WriteLine("recv null message, err:" + packet.ErrorCode() + " cmd:" + packet.Command());
                    continue;
                }
                if (!HandlerRegister.OnRecvPacket(packet))
                {
                    // 没注册的消息
                    if (packet.ErrorCode() > 0)
                    {
                        Console.WriteLine("recv err:" + packet.ErrorCode() + " msg:" + packet.Message());
                    }
                    else
                    {
                        Console.WriteLine("recv cmd:" + packet.Command() + " msg:" + packet.Message());
                    }
                }
            }
        }
    }
}
