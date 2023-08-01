using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

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
        public string puzzleRewards { get; set; }
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
            puzzleRewards = string.Empty;
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

        public IPacket Read(ByteBuffer buffer, string json = "")
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            foreach (var (key,value) in dict)
            {
                var keyString = key.ToString();
                switch (keyString)
                {
                    case "":
                    {
                        
                    }
                        break;
                }
            }

            return null;
        }
    }
}