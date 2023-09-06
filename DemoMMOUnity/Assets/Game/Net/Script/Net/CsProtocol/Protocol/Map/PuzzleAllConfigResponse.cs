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
        public List<PuzzleChapter> PuzzleChaptersList = new List<PuzzleChapter>();

        public short ProtocolId()
        {
            return 1036;
        }

        public void Clear()
        {
            if (PuzzleList != null)
            {
                PuzzleList.Clear();
            }
            PuzzleList = new List<Puzzle>();
            if (PuzzleChaptersList != null)
            {
                PuzzleChaptersList.Clear();
            }
            PuzzleChaptersList = new List<PuzzleChapter>();
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
            packet.Clear();
            foreach (var (key,value) in dict)
            {
                var keyString = key.ToString();
                var valueString = value.ToString();
                switch (keyString)
                {
                    case "puzzleConfigList":
                    {
                        var packetDict = JsonConvert.DeserializeObject<List<object>>(valueString);
                     
                        int length = packetDict.Count;
                        for (int i = 0; i < length; i++)
                        {
                            var dataObj = packetDict[i];
                            var dataString = dataObj.ToString();
                            var puzzleData = ProtocolManager.GetProtocol(202);
                            var puzzleRead = (Puzzle)puzzleData.Read(dataString);
                            packet.PuzzleList.Add(puzzleRead);
                        }
                    }
                        break;
                    case "puzzleChapterConfigList":
                    {
                        var packetDict = JsonConvert.DeserializeObject<List<object>>( valueString);
                        var length = packetDict.Count;
                        for (var i = 0; i < length; i++)
                        {
                            var dataObj = packetDict[i];
                            var dataString = dataObj.ToString();
                            var puzzleData = ProtocolManager.GetProtocol(204);
                            var puzzleRead = (PuzzleChapter)puzzleData.Read(dataString);
                            packet.PuzzleChaptersList.Add(puzzleRead);
                        }
                    }
                        break;
                }
            }
            return packet;
        }
    }
}