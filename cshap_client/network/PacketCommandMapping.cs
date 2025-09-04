using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using gnet_csharp;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Gserver;
using Newtonsoft.Json;

namespace cshap_client.network
{
    // 消息和消息号的映射
    public static class PacketCommandMapping
    {
        private static readonly Dictionary<Type, ushort> _messageTypeCmdMapping = new Dictionary<Type, ushort>();
        private static readonly Dictionary<ushort, MessageDescriptor> _cmdMessageMapping = new Dictionary<ushort, MessageDescriptor>();
        private static TypeRegistry _typeRegistry; // 所有注册的messageDescriptor

        // 对应proto文件里的package导出名
        public const string ProtoPackageName = "gserver";
        
        // 根据消息体结构查找对应的消息号
        public static ushort GetCommandByProto(IMessage protoMessage)
        {
            if (protoMessage == null)
            {
                Console.WriteLine("GetCommandByProto: protoMessage is null");
                return 0;
            }

            var type = protoMessage.GetType();
            if (_messageTypeCmdMapping.TryGetValue(type, out ushort cmd))
            {
                return cmd;
            }

            Console.WriteLine("GetCommandByProtoErr: {0}", type.Name);
            return 0;
        }

        // 加载消息映射文件,建立消息和消息号的映射关系
        public static void InitCommandMappingFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("InitCommandMappingFromFile: filePath is null or empty");
                return;
            }

            var mapping = LoadCommandMapping(filePath);
            if (mapping == null)
            {
                Console.WriteLine("InitCommandMappingFromFile: Failed to load {0}", filePath);
                return;
            }
            _typeRegistry = TypeRegistry.FromFiles(GlobalFileDescriptorSet.Descriptors);

            foreach (var kvp in mapping)
            {
                string messageName = kvp.Key;
                ushort messageId = (ushort)kvp.Value;
                string fullMessageName = GetFullMessageName(messageName);
                var messageDescriptor = _typeRegistry.Find(fullMessageName);
                if (messageDescriptor == null)
                {
                    Console.WriteLine("FindMessageTypeErr: Cannot find messageDescriptor {0}", messageName);
                    continue;
                }
                // 查找匹配的消息类型
                var messageType = messageDescriptor.ClrType;
                if (messageType == null)
                {
                    Console.WriteLine("FindMessageTypeErr: Cannot find message type for {0}", messageName);
                    continue;
                }
                // 创建消息实例以获取描述符
                try
                {
                    var messageInstance = Activator.CreateInstance(messageType) as IMessage;
                    if (messageInstance != null)
                    {
                        _messageTypeCmdMapping[messageType] = messageId;
                        _cmdMessageMapping[messageId] = messageDescriptor;
                        Console.WriteLine("CommandMapping msg:{0} id:{1}", messageName, messageId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static Dictionary<string, int> LoadCommandMapping(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("LoadCommandMappingErr: File not found {0}", filePath);
                return null;
            }

            try
            {
                string jsonContent = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static string GetFullMessageName(string messageName)
        {
            if (string.IsNullOrEmpty(ProtoPackageName))
            {
                return messageName;
            }
            return ProtoPackageName + "." + messageName;
        }

        // 根据消息号查找消息类型
        public static MessageDescriptor GetMessageDescriptorByCommand(ushort commandId)
        {
            if (!_cmdMessageMapping.TryGetValue(commandId, out MessageDescriptor messageDescriptor))
            {
                return null;
            }
            return messageDescriptor;
        }

        public static MessageDescriptor GetMessageDescriptor(string messageName)
        {
            return _typeRegistry.Find(GetFullMessageName(messageName));
        }

        // 创建消息号对应的消息实例
        public static IMessage CreateMessageByCommand(ushort commandId)
        {
            var messageType = GetMessageDescriptorByCommand(commandId);
            if (messageType == null)
            {
                return null;
            }

            try
            {
                return Activator.CreateInstance(messageType.ClrType) as IMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // 注册消息到网络层解码器
        public static void RegisterCodec(ICodec codec)
        {
            switch (codec)
            {
             case ProtoCodec c:
                 foreach(var kvp in _cmdMessageMapping)
                 {
                     c.Register((ushort)kvp.Key, kvp.Value);
                 }
                 break;
             
             case SimpleProtoCodec c:
                 foreach(var kvp in _cmdMessageMapping)
                 {
                     c.Register((ushort)kvp.Key, kvp.Value);
                 }
                 break;
            }
        }
    }
}