using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Map
{
    public class Puzzle : Model, IPacket, IReference
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        public string puzzleName { get; set; }
        public int lastPuzzleID { get; set; }
        public int nextPuzzleID { get; set; }
        public PuzzleRewardsData[] puzzleRewards { get; set; }
        public string icon { get; set; }
        public string resourcePath { get; set; }

        public short ProtocolId()
        {
            return 202;
        }

        public void Clear()
        {
            id = 0;
            puzzleName = string.Empty;
            lastPuzzleID = 0;
            nextPuzzleID = 0;
            puzzleRewards = Array.Empty<PuzzleRewardsData>();
            icon = string.Empty;
            resourcePath = string.Empty;
        }

        public static Puzzle ValueOf()
        {
            var packet = ReferenceCache.Acquire<Puzzle>();
            packet.Clear();
            return packet;
        }

        public static Puzzle ValueOf(int id, string puzzleName, int lastPuzzleID,
            int nextPuzzleID, string puzzleRewards, string icon, string resourcePath)
        {
            var packet = ReferenceCache.Acquire<Puzzle>();
            packet.Clear();
            return packet;
        }
    }

    public class PuzzleRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 202;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var data = (Puzzle)packet;
            var packetData = new ServerMessageWrite(protocolId: ProtocolId(), data);
            var jsonStr = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(jsonStr);
        }

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var packet = Puzzle.ValueOf();
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            foreach (var (key, value) in dict)
            {
                var keyString = key.ToString();
                var valueString = value.ToString();
                switch (keyString)
                {
                    case "id":
                    {
                        var id = int.Parse(valueString);
                        packet.id = id;
                    }
                        break;
                    case "puzzleName":
                    {
                        packet.puzzleName = valueString;
                    }
                        break;
                    case "lastPuzzleID":
                    {
                        var lastPuzzleID = int.Parse(valueString);
                        packet.lastPuzzleID = lastPuzzleID;
                    }
                        break;
                    case "nextPuzzleID":
                    {
                        var nextPuzzleID = int.Parse(valueString);
                        packet.nextPuzzleID = nextPuzzleID;
                    }
                        break;
                    case "puzzleRewards":
                    {
                        var rewardDict = JsonConvert.DeserializeObject<List<Object>>(valueString);
                        int length = rewardDict.Count;
                        packet.puzzleRewards = new PuzzleRewardsData[length];
                        var byteBuffer = ByteBuffer.ValueOf();
                        for (int i = 0; i < length; i++)
                        {
                            var data1 = rewardDict[i].ToString();
                            var packetDataRegistration = ProtocolManager.GetProtocol(203);
                            byteBuffer.WriteString(data1);
                            var packetDataRead = packetDataRegistration.Read(byteBuffer);
                            var packetData = (PuzzleRewardsData)packetDataRead;
                            packet.puzzleRewards[i] = packetData;
                            byteBuffer.Clear();
                        }
                    }
                        break;
                    case "icon":
                    {
                        packet.icon = valueString;
                    }
                        break;
                    case "resourcePath":
                    {
                        packet.resourcePath = valueString;
                    }
                        break;
                }
            }

            return packet;
        }
    }
}