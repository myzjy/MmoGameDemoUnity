using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Module.PuzzleNet.Model;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Map
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

        public IPacket Read(string json = "")
        {
            var packet = PuzzleRewardsData.ValueOf();
            packet.Clear();
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            //解析
            var dict = JsonConvert.DeserializeObject<Dictionary<Object, Object>>(json);
            foreach (var (key,value) in dict)
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
                    
                }
            }

            return packet;
        }
    }
}