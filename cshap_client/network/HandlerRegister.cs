using gnet_csharp;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace cshap_client.network
{
    // 消息回调注册
    internal class HandlerRegister
    {
        // 规范: 消息回调函数名格式: OnXyz(Xyz res) 或者 OnXyz(Xyz res, int errCode)
        public const string HandlerMethodPrefix = "On";

        // 注册的消息回调
        private static Hashtable m_Handlers = new Hashtable();

        // 扫描一个类的所有函数,自动注册消息回调
        public static void RegisterMethodsForClass(Type type)
        {
            // 获取所有方法
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                if (!method.Name.StartsWith(HandlerMethodPrefix))
                {
                    continue;
                }
                var messageName = method.Name.Substring(HandlerMethodPrefix.Length);
                //var descriptor = PacketCommandMapping.GetMessageDescriptor(messageName);
                //if (descriptor == null)
                //{
                //    continue;
                //}
                if (method.GetParameters().Length < 1)
                {
                    continue;
                }
                var messageParamInfo = method.GetParameters()[0];
                if (!typeof(IMessage).IsAssignableFrom(messageParamInfo.ParameterType))
                {
                    continue;
                }
                if (messageParamInfo.ParameterType.Name != messageName)
                {
                    Console.WriteLine("messageName not matched:" + method.Name + " messageName:" + messageName + " paramName:" + messageParamInfo.ParameterType.Name);
                    continue;
                }
                m_Handlers.Add(messageParamInfo.ParameterType, method);
                Console.WriteLine("RegisterHandler:" + method.Name + " message:" + messageName);
            }
        }

        public static bool OnRecvPacket(IPacket packet)
        {
            var message = packet.Message();
            try
            {
                object obj = m_Handlers[message.GetType()];
                if (obj == null)
                {
                    Console.WriteLine("not find method, message" + message.GetType().Name);
                    return false;
                }
                var method = obj as MethodInfo;
                object[] parameters = new object[] { message };
                if (method.GetParameters().Length == 2)
                {
                    parameters = new object[] { message, (int)packet.ErrorCode() };
                }
                if (method.IsStatic)
                {
                    method.Invoke(null, parameters);
                }
                else
                {
                    method.Invoke(obj, parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("OnRecvPacketErr: message" + message.GetType().Name + " ex:" + ex.Message);
                return false;
            }
            return true;
        }
    }
}
