using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Map
{
    /// <summary>
    /// 地图 章节数据
    /// </summary>
    public class PuzzleChapter : Model, IPacket, IReference
    {
        /**
         * id
         */
        public int id { get; set; }

        /**
         * 章节名
         */
        public string chapterName { get; set; }

        /**
         * 当前章节 最小 关卡id
         */
        public int minPuzzle { get; set; }

        /**
         * 当前章节 最大 关卡id
         */
        public int maxPuzzle { get; set; }

        /**
         * 当前 进行中的 关卡id
         * <p>
         * 如果这个数字 为负数就查看 doneMaxPuzzleId
         * 是否有值，没有值就代表 没有进行
         * </p>
         */
        public int nowCarryOutPuzzleId { get; set; }

        /**
         * 完成的最大关卡id 只有 关卡完成之后 才会更新
         */
        public int doneMaxPuzzleId { get; set; }

        public short ProtocolId()
        {
            return 204;
        }

        public void Clear()
        {
            id = -1;
            chapterName = string.Empty;
            minPuzzle = -1;
            maxPuzzle = -1;
            nowCarryOutPuzzleId = -1;
            doneMaxPuzzleId = -1;
        }

        public static PuzzleChapter ValueOf()
        {
            var data = ReferenceCache.Acquire<PuzzleChapter>();
            return data;
        }
    }

    public class PuzzleChapterRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 204;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var data = (PuzzleChapter)packet;
            var packetData = new ServerMessageWrite(protocolId: ProtocolId(), data);
            var jsonString = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(jsonString);
        }

        public IPacket Read(string json = "")
        {
            var packet = PuzzleChapter.ValueOf();
            packet.Clear();
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            //解析
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            foreach (var (key, value) in dict)
            {
                var keyStr = key.ToString();
                var valueStr = value.ToString();
                switch (keyStr)
                {
                    case "id":
                    {
                        var id = int.Parse(valueStr);
                        packet.id = id;
                    }
                        break;
                    case "chapterName":
                    {
                        packet.chapterName = valueStr;
                    }
                        break;
                    case "minPuzzle":
                    {
                        var minPuzzle = int.Parse(valueStr);
                        packet.minPuzzle = minPuzzle;
                    }
                        break;
                    case "maxPuzzle":
                    {
                        var maxPuzzle = int.Parse(valueStr);
                        packet.maxPuzzle = maxPuzzle;
                    }
                        break;
                    case "nowCarryOutPuzzleId":
                    {
                        var nowCarryOutPuzzleId = int.Parse(valueStr);
                        packet.nowCarryOutPuzzleId = nowCarryOutPuzzleId;
                    }
                        break;
                    case "doneMaxPuzzleId":
                    {
                        var doneMaxPuzzleId = int.Parse(valueStr);
                        packet.doneMaxPuzzleId = doneMaxPuzzleId;
                    }
                        break;
                }
            }

            return packet;
        }
    }
}