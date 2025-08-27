using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cshap_client.network;
using gnet_csharp;
using Google.Protobuf;
using Google.Protobuf.Reflection;

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

        public void Run()
        {
            while (IsRunning)
            {
                m_Connection.AutoPing();
                if (m_InputCmds.TryDequeue(out string cmd))
                {
                    OnCommand(cmd);
                }
                m_Connection.ProcessPackets();
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

        // 测试命令
        public void OnCommand(string cmd)
        {
            Console.WriteLine("OnCommand:" + cmd);
            var cmdArgs = cmd.Split(' ');
            if (cmdArgs.Length == 0)
            {
                return;
            }
            if (cmdArgs[0] == "xxx")
            {
                // TODO: 特殊的测试命令
            }
            else
            {
                // 通用的protobuf消息
                // 格式: messageName fieldName fieldValue fieldName fieldValue ...
                var messageName = cmdArgs[0];
                var messageDescriptor = PacketCommandMapping.GetMessageDescriptor(messageName);
                if (messageDescriptor == null)
                {
                    Console.WriteLine("not find message:" + messageName);
                    return;
                }
                // 创建一个新消息
                var message = Activator.CreateInstance(messageDescriptor.ClrType) as IMessage;
                for (int i = 1; i < cmdArgs.Length && i+1 < cmdArgs.Length; i+=2)
                {
                    var fieldName = cmdArgs[i];
                    var fieldValue = cmdArgs[i+1];
                    var fieldDescriptor = messageDescriptor.FindFieldByName(fieldName);
                    if (fieldDescriptor == null )
                    {
                        Console.WriteLine("not find fieldName:" + fieldName);
                        continue;
                    }
                    switch(fieldDescriptor.FieldType)
                    {
                        case FieldType.String:
                            fieldDescriptor.Accessor.SetValue(message, fieldValue);
                            break;
                        case FieldType.Int32:
                            var i32 = Int32.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, i32);
                            break;
                        case FieldType.UInt32:
                            var u32 = UInt32.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, u32);
                            break;
                        case FieldType.Int64:
                            var i64 = Int64.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, i64);
                            break;
                        case FieldType.UInt64:
                            var u64 = UInt64.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, u64);
                            break;
                        case FieldType.Bool:
                            var lowerValue = fieldValue.ToLower();
                            var b = (lowerValue == "1" || lowerValue == "true");
                            fieldDescriptor.Accessor.SetValue(message, b);
                            break;
                        case FieldType.Float:
                            var f = float.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, f);
                            break;
                        case FieldType.Double:
                            var d = double.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, d);
                            break;
                        default:
                            Console.WriteLine("unsupported FieldType:" + fieldDescriptor.FieldType);
                            break;
                    }
                    
                }
                if (!m_Connection.Send(message))
                {
                    Console.WriteLine("send err:" + messageName);
                }
            }
        }
    }
}
