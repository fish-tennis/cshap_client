using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gnet_csharp;

namespace cshap_client.game
{
    internal class TestClient
    {
        public TcpConnection m_Connection;
        public TestClient()
        {
            var connectionConfig = new ConnectionConfig
            {
                RecvBufferSize = 1024 * 100,
                RecvTimeout = 1000,
                WriteTimeout = 1000
            };
            var codec = new ProtoCodec();
            connectionConfig.Codec = codec;
            m_Connection = new TcpConnection(connectionConfig, 1)
            {
                Tag = this,
                OnConnected = onConnected,
                OnClose = onClose,
            };

            //codec.Register(Convert.ToUInt16(pb.CmdTest.CmdHeartBeat), pb.HeartBeatRes.Descriptor);
            //codec.Register(Convert.ToUInt16(pb.CmdTest.Message), pb.TestMessage.Descriptor);
        }

        public void Start()
        {
            m_Connection.Connect("127.0.0.1:10002");
        }

        public void Stop()
        {
            m_Connection.Close();
        }

        private void onConnected(IConnection connection, bool success)
        {
            Console.WriteLine("onConnected host:" + m_Connection.GetHostAddress() + " success:" + success);
        }

        private void onClose(IConnection connection)
        {
            Console.WriteLine("onClose");
        }

        public void ProcessPackets()
        {
            while (true)
            {
                var packet = m_Connection.PopPacket();
                if (packet == null)
                {
                    return;
                }
                // write your logic code here
                Console.WriteLine("recv cmd:" + packet.Command() + " msg:" + packet.Message());
            }
        }
    }
}
