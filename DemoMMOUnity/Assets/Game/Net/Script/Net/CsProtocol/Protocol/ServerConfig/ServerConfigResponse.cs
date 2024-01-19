using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.CsProtocol.Protocol.Bag;
using ZJYFrameWork.Net.CsProtocol.Protocol.Equipment;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.ServerConfig
{
    /// <summary>
    /// 系统基础配置表相关
    /// </summary>
    public class ServerConfigResponse : Model, IPacket, IReference
    {
        public List<ItemBaseData> bagItemEntityList { get; set; } = default;

        /**
         * 装备基础相关 根据品节 卡等级 强化获取副属性条
        */
        public List<EquipmentConfigBaseData> equipmentConfigBaseDataList { get; set; } = default;

        /**
         * 圣遗物基础配置相关
         */
        public List<EquipmentBaseData> equipmentBaseDataList { get; set; } = default;

        /**
         * 圣遗物位置信息
         */
        public List<EquipmentPrimaryConfigBaseData> equipmentPrimaryConfigBaseDataList { get; set; } = default;

        /**
         * 圣遗物介绍
         */
        public List<EquipmentDesBaseData> equipmentDesBaseDataList { get; set; } = default;

        /**
         * 圣遗物位置名字
         */
        public List<EquipmentGrowthConfigBaseData> equipmentGrowthConfigBaseDataList { get; set; } = default;

        /**
         * 圣遗物副属性
         */
        public List<EquipmentGrowthViceConfigBaseData> equipmentGrowthViceConfigBaseDataList { get; set; } = default;

        public short ProtocolId()
        {
            return 1010;
        }

        public static ServerConfigResponse ValueOf()
        {
            var packet = ReferenceCache.Acquire<ServerConfigResponse>();
            packet.Clear();
            return packet;
        }

        public static ServerConfigResponse ValueOf(List<ItemBaseData> bagItemEntityList)
        {
            var packet = new ServerConfigResponse
            {
                bagItemEntityList = bagItemEntityList
            };
            return packet;
        }

        public void Clear()
        {
            bagItemEntityList = new List<ItemBaseData>();
            if (equipmentConfigBaseDataList != null)
            {
                equipmentConfigBaseDataList.Clear();
            }
            else
            {
                equipmentConfigBaseDataList = new List<EquipmentConfigBaseData>();
            }

            equipmentBaseDataList = new List<EquipmentBaseData>();
            equipmentPrimaryConfigBaseDataList = new List<EquipmentPrimaryConfigBaseData>();
            equipmentDesBaseDataList = new List<EquipmentDesBaseData>();
            equipmentGrowthConfigBaseDataList = new List<EquipmentGrowthConfigBaseData>();
            equipmentGrowthViceConfigBaseDataList = new List<EquipmentGrowthViceConfigBaseData>();
        }
    }

    public class ServerConfigResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1010;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            var message = (ServerConfigResponse)packet;
            var messageWrite = new ServerMessageWrite(message.ProtocolId(), message);
            var json = JsonConvert.SerializeObject(messageWrite);
            buffer.WriteString(json);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = ServerConfigResponse.ValueOf();
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            foreach (var (key, value) in dict)
            {
                var byteBuffers = ByteBuffer.ValueOf();
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "bagItemEntityList":
                    {
                        var valueString = value.ToString();
                        var bagEntityList = JsonConvert.DeserializeObject<List<object>>(valueString);
                        int length = bagEntityList.Count;
                        packet.bagItemEntityList = new List<ItemBaseData>();
                        for (int i = 0; i < length; i++)
                        {
                            var entityObj = bagEntityList[i];
                            var packetData = ProtocolManager.GetProtocol(201);
                            byteBuffers.WriteString(entityObj.ToString());
                            var packetDataRead = (ItemBaseData)packetData.Read(byteBuffers);
                            packet.bagItemEntityList.Add(packetDataRead);
                            byteBuffers.Clear();
                        }
                    }
                        break;
                    case "equipmentBaseDataList":
                    {
                        var valueString = value.ToString();
                        var entityList = JsonConvert.DeserializeObject<List<object>>(valueString);
                        int length = entityList.Count;
                        for (int i = 0; i < length; i++)
                        {
                            var obj = entityList[i];
                            var packetData = ProtocolManager.GetProtocol(EquipmentBaseData.GetProtocolId());
                            byteBuffers.WriteString(obj.ToString());
                            var createPacket = (EquipmentBaseData)packetData.Read(byteBuffers);
                            packet.equipmentBaseDataList.Add(createPacket);
                            byteBuffers.Clear();
                        }
                    }
                        break;
                    case "equipmentConfigBaseDataList":
                    {
                        var valueString = value.ToString();
                        var entityList = JsonConvert.DeserializeObject<List<object>>(valueString);
                        int length = entityList.Count;
                        for (int i = 0; i < length; i++)
                        {
                            var obj = entityList[i];
                            var packetData = ProtocolManager.GetProtocol(EquipmentConfigBaseData.GetProtocolId());
                            byteBuffers.WriteString(obj.ToString());
                            var createPacket = (EquipmentConfigBaseData)packetData.Read(byteBuffers);
                            packet.equipmentConfigBaseDataList.Add(createPacket);
                            byteBuffers.Clear();
                        }
                    }
                        break;
                    case "equipmentPrimaryConfigBaseDataList":
                    {
                        var valueString = value.ToString();
                        var entityList = JsonConvert.DeserializeObject<List<object>>(valueString);
                        int length = entityList.Count;
                        for (int i = 0; i < length; i++)
                        {
                            var obj = entityList[i];
                            var packetData = ProtocolManager.GetProtocol(EquipmentPrimaryConfigBaseData.GetProtocolId());
                            byteBuffers.WriteString(obj.ToString());
                            var createPacket = (EquipmentPrimaryConfigBaseData)packetData.Read(byteBuffers);
                            packet.equipmentPrimaryConfigBaseDataList.Add(createPacket);
                            byteBuffers.Clear();
                        }
                    }
                        break;
                    case "equipmentDesBaseDataList":
                    {
                        var valueString = value.ToString();
                        var entityList = JsonConvert.DeserializeObject<List<object>>(valueString);
                        int length = entityList.Count;
                        for (int i = 0; i < length; i++)
                        {
                            var obj = entityList[i];
                            var packetData = ProtocolManager.GetProtocol(EquipmentDesBaseData.GetProtocolId());
                            byteBuffers.WriteString(obj.ToString());
                            var createPacket = (EquipmentDesBaseData)packetData.Read(byteBuffers);
                            packet.equipmentDesBaseDataList.Add(createPacket);
                            byteBuffers.Clear();
                        }
                    }
                        break;
                    case "equipmentGrowthConfigBaseDataList":
                    {
                        var valueString = value.ToString();
                        var entityList = JsonConvert.DeserializeObject<List<object>>(valueString);
                        int length = entityList.Count;
                        for (int i = 0; i < length; i++)
                        {
                            var obj = entityList[i];
                            var packetData = ProtocolManager.GetProtocol(EquipmentGrowthConfigBaseData.GetProtocolId());
                            byteBuffers.WriteString(obj.ToString());
                            var createPacket = (EquipmentGrowthConfigBaseData)packetData.Read(byteBuffers);
                            packet.equipmentGrowthConfigBaseDataList.Add(createPacket);
                            byteBuffers.Clear();
                        }
                    }
                        break;
                    case "equipmentGrowthViceConfigBaseDataList":
                    {
                        var valueString = value.ToString();
                        var entityList = JsonConvert.DeserializeObject<List<object>>(valueString);
                        int length = entityList.Count;
                        for (int i = 0; i < length; i++)
                        {
                            var obj = entityList[i];
                            var packetData = ProtocolManager.GetProtocol(EquipmentGrowthViceConfigBaseData.GetProtocolId());
                            byteBuffers.WriteString(obj.ToString());
                            var createPacket = (EquipmentGrowthViceConfigBaseData)packetData.Read(byteBuffers);
                            packet.equipmentGrowthViceConfigBaseDataList.Add(createPacket);
                            byteBuffers.Clear();
                        }
                    }
                        break;
                }
            }

            return packet;
        }
    }
}