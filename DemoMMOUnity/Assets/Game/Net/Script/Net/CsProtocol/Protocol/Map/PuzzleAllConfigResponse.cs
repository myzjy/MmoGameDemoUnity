using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Map
{
    /// <summary>
    /// 地图 信息 response
    /// </summary>
    public class PuzzleAllConfigResponse : Model, IPacket, IReference
    {
        public List<Puzzle> PuzzleList = new List<Puzzle>();

        public short ProtocolId()
        {
            return 1036;
        }

        public void Clear()
        {
            PuzzleList = new List<Puzzle>();
        }

        public static PuzzleAllConfigResponse ValueOf()
        {
            var packet = ReferenceCache.Acquire<PuzzleAllConfigResponse>();
            packet.Clear();
            return packet;
        }
    }

    public class PuzzleAllConfigResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1036;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var data = (PuzzleAllConfigResponse)packet;
            var packetData = new ServerMessageWrite(protocolId: ProtocolId(), data);
            var jsonString = JsonConvert.SerializeObject(packetData);
            buffer.WriteString(jsonString);
        }

        public IPacket Read(string json = "")
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            var packet = PuzzleAllConfigResponse.ValueOf();

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            dict.TryGetValue("puzzleConfigList", out var listObj);
            if (listObj == null) return packet;
            var dictString = listObj.ToString();
            var packetDict = JsonConvert.DeserializeObject<List<object>>(dictString);
            packet.Clear();
            int length = packetDict.Count;
            for (int i = 0; i < length; i++)
            {
                var dataObj = packetDict[i];
                var dataString = dataObj.ToString();
                var puzzleData = ProtocolManager.GetProtocol(202);
                var puzzleRead = (Puzzle)puzzleData.Read(dataString);
                packet.PuzzleList.Add(puzzleRead);
            }

            return packet;
        }
    }
}