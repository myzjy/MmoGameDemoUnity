using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Module.PuzzleNet.Model;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.Map
{
    public class PuzzleRewardsData : Model, IPacket, IReference
    {
        /**
 * 奖励的物品id
 */
        public int rewardId { get; set; }

        /**
         * 奖励 type
         */
        public PuzzleRewardsEnum rewardType { get; set; }

        /**
         * 奖励icon
         */
        public string rewardIcon { get; set; }

        /**
         * 奖励 资源
         */
        public string rewardResource { get; set; }

        /**
         * 奖励数量
         */
        public int num { get; set; }

        public short ProtocolId()
        {
            return 203;
        }

        public void Clear()
        {
            rewardId = 0;
            rewardType = 0;
            rewardIcon = string.Empty;
            rewardResource = string.Empty;
            num = 0;
        }

        public static PuzzleRewardsData ValueOf()
        {
            var packet = ReferenceCache.Acquire<PuzzleRewardsData>();
            packet.Clear();
            return packet;
        }
    }

    public class PuzzleRewardsDataRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 203;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            throw new NotImplementedException();
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = PuzzleRewardsData.ValueOf();
            packet.Clear();
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            //解析
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            foreach (var (key, value) in dict)
            {
                var keyStr = key.ToString();
                var valueStr = value.ToString();
                switch (keyStr)
                {
                    case "rewardId":
                    {
                        packet.rewardId = int.Parse(valueStr);
                    }
                        break;
                    case "rewardType":
                    {
                        var typeNum = int.Parse(valueStr);
                        packet.rewardType = (PuzzleRewardsEnum)typeNum;
                    }
                        break;
                    case "rewardIcon":
                    {
                        packet.rewardIcon = valueStr;
                    }
                        break;
                    case "rewardResource":
                    {
                        packet.rewardResource = valueStr;
                    }
                        break;
                }
            }

            return packet;
        }
    }
}