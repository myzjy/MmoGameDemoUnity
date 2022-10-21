using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    /**
     * 协议的注册管理类
     * 
     */
    public class ProtocolManager
    {
        /**
         * 协议号类的最大个数
         */
        public static readonly short MAX_PROTOCOL_NUM = short.MaxValue;

        /**
        * 当前项目中所有继承IProtocolRegistration的类
        */
        private static readonly IProtocolRegistration[] protocolList = new IProtocolRegistration[MAX_PROTOCOL_NUM];


        /**
         * 初始化项目内协议类
         */
        public static void InitProtocol()
        {
            //记录项目中所有protocolRegistration类
            var protocolRegistrationTypeList = new List<Type>();
            var typeList = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var itemAssemblies in typeList)
            {
                //在ProtocolManager类的命名空间下的所有类，进行扫描
                if (!itemAssemblies.Equals(typeof(ProtocolManager).Assembly)) continue;
                var results = new List<Type>();
                //添加当前命名空间下的所有类
                results.AddRange(itemAssemblies.GetTypes());
                //进行筛选之后进行添加
                //要求：
                //1.必须是class;
                //2.不能带有abstract关键字（不能是抽象类）
                //3.继承IProtocolRegistration 接口是否存在当前命名空间下
                protocolRegistrationTypeList.AddRange(results.Where(type =>
                    type.IsClass && !type.IsAbstract && typeof(IProtocolRegistration).IsAssignableFrom(type)));
            }

            foreach (var protocolRegistration in protocolRegistrationTypeList.Select(protocolType =>
                         (IProtocolRegistration)Activator.CreateInstance(protocolType)))
            {
                protocolList[protocolRegistration.ProtocolId()] = protocolRegistration;
            }
        }

        public static IProtocolRegistration GetProtocol(short protocolId)
        {
            var protocol = protocolList[protocolId];
            if (protocol == null)
            {
                throw new Exception("[protocolId:" + protocolId + "]协议不存在");
            }

            return protocol;
        }

        public static void Write(ByteBuffer byteBuffer, IPacket packet)
        {
            var protocolId = packet.ProtocolId();
            // // 写入协议号
            // byteBuffer.WriteShort(protocolId);

            // 写入包体
            GetProtocol(protocolId).Write(byteBuffer, packet);
        }

        public static IPacket Read(ByteBuffer byteBuffer)
        {
            ;
            var json = StringUtils.BytesToString(byteBuffer.ToBytes());
            var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);


            jsonDict.TryGetValue("protocolId", out var protocolStr);

            if (protocolStr != null)
            {
                var protocolId = short.Parse(protocolStr);
                return GetProtocol(protocolId).Read(byteBuffer);
            }

            Debug.Log("接受消息协议出错");

            return null;
        }
    }
}