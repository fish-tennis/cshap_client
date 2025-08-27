using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cshap_client.network;
using gnet_csharp;

namespace cshap_client.game
{
    // 单件模式的客户端对象,放一些全局数据
    internal class Client
    {
        private static Client _instance = null;

        public ClientConnection m_Connection;
        public bool IsRunning = false;
        private ConcurrentQueue<string> m_InputCmds = new ConcurrentQueue<string>();

        private Client() { }
        public static Client Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Client();
                }
                return _instance;
            }
        }

        public void Init()
        {
            PacketCommandMapping.InitCommandMappingFromFile("gen/message_command_mapping.json");
            var connectionConfig = new ConnectionConfig
            {
                RecvBufferSize = 1024 * 100,
                RecvTimeout = 3000,
                WriteTimeout = 3000
            };
            var codec = new ProtoCodec();
            connectionConfig.Codec = codec;
            PacketCommandMapping.RegisterCodec(codec); // 自动注册所有消息
            m_Connection = new ClientConnection(connectionConfig, 1);
            IsRunning = true;
        }

        public void ProcessPackets()
        {
            while (IsRunning)
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

        public void Run()
        {
            while (IsRunning)
            {
                m_Connection.AutoPing();
                if (m_InputCmds.TryDequeue(out string cmd))
                {
                    OnCommand(cmd);
                }
                ProcessPackets();
                Thread.Sleep(50);
            }
        }

        public void Shutdown()
        {
            m_Connection.Close();
        }

        // 从其他线程收到cmd
        public void RecvCommand(string cmd)
        {
            m_InputCmds.Enqueue(cmd);
        }

        public void OnCommand(string cmd)
        {
            Console.WriteLine("OnCommand:" + cmd);
        }
    }
}
