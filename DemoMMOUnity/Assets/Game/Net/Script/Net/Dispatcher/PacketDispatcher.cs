using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Net.Dispatcher
{
    /// <summary>
    /// 网络包注册
    /// </summary>
    public abstract class PacketDispatcher
    {
        /**
         * 所有注册的网络协议包
         */
        private static readonly Dictionary<Type, IPacketReceiver> _receiversMap =
            new Dictionary<Type, IPacketReceiver>();

        /**
         * 初始化网络协议
         * 扫描内部类 进行注册网络协议
         */
        public static void Scan()
        {
            //初始化
            ProtocolManager.InitProtocol();
            //项目中所有带有bean注解的类
            var allBeans = SpringContext.GetAllBeans();
            foreach (var item in allBeans)
            {
                RegisterPacketReceiver(item);
            }
        }

        private static void RegisterPacketReceiver([NotNull] object bean)
        {
            var classType = bean.GetType();
            var methods = AssemblyUtils.GetMethodsByAnnoInPojoClass(classType, typeof(PacketReceiverAttribute));
            foreach (var itMethod in methods)
            {
                var parameters = itMethod.GetParameters();

                if (parameters.Length != 1)
                {
                    throw new Exception(StringUtils.Format("[class:{}] [method:{}] 必须有一个参数!",
                        bean.GetType().Name, itMethod.Name));
                }

                if (!typeof(IPacket).IsAssignableFrom(parameters[0].ParameterType))
                {
                    throw new Exception(StringUtils.Format("[class:{}] [method:{}] 必须有一个[IPacket]类型参数!",
                        bean.GetType().Name, itMethod.Name));
                }

                var paramType = itMethod.GetParameters()[0].ParameterType;
                var expectedMethodName = StringUtils.Format("At{}", paramType.Name);
                if (!itMethod.Name.Equals(expectedMethodName))
                {
                    throw new Exception(StringUtils.Format(
                        "[class:{}] [method:{}] [event:{}] expects '{}' as method name!"
                        , bean.GetType().FullName, itMethod.Name, paramType.Name, expectedMethodName));
                }

                var receiverDefinition = PacketReceiverDefinition.ValueOf(bean, itMethod);
                if (_receiversMap.ContainsKey(paramType))
                {
                    throw new Exception(StringUtils.Format("消息接收器重复[{}][{}]", bean.GetType().Name, paramType.Name));
                }

                _receiversMap.Add(paramType, receiverDefinition);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="packet"></param>
        public static void Send(IPacket packet)
        {
        }

        public static void Receive(IPacket packet)
        {
            if (packet == null)
            {
                Debug.Log("收到消息为空");
                return;
            }

            _receiversMap.TryGetValue(packet.GetType(), out var packetReceiver);
            if (packetReceiver == null)
            {
                Debug.LogError($"没有可以接收消息[{packet.GetType().FullName}]的接收器[{JsonConvert.SerializeObject(packet)}]");
                return;
            }

            packetReceiver.Invoke(packet);
        }
        
    }
}