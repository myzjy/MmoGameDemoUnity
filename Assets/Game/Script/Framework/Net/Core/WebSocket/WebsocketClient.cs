using System.Collections.Generic;
using BestHTTP.WebSocket;
using Newtonsoft.Json;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.Core.Websocket
{
    public class WebsocketClient : AbstractClient
    {
        private string url;
        private WebSocketBridge _webSocketBridge = new WebSocketBridge();

        public WebsocketClient(string url)
        {
            this.url = url;
        }

        public override void Start()
        {
            if (!_webSocketBridge.initialized)
            {
            }
        }

        public override bool Connected()
        {
            throw new System.NotImplementedException();
        }

        public override void Close()
        {
            throw new System.NotImplementedException();
        }

        public override string ToConnectUrl()
        {
            throw new System.NotImplementedException();
        }

        protected override void SendMessagesBlocking(byte[] messages, int offset, int size)
        {
            throw new System.NotImplementedException();
        }

        protected override bool ReadMessageBlocking(out byte[] content)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 打开回调
        /// </summary>
        internal void HandleOnOpen(WebSocket ws)
        {
        }

        internal void HandleOnMessage(byte[] content)
        {
            var json = StringUtils.BytesToString(content);
          
            var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            
            
            
            jsonDict.TryGetValue("protocolId", out var protocolStr);
            
            if (protocolStr != null)
            {
                var protocolId = short.Parse(protocolStr);
                jsonDict.TryGetValue("packet",out var packetString);
                // var packetStr = 
            }

            // var byteBuffer = ByteBuffer.ValueOf();
            // byteBuffer.WriteBytes(content);

            // byteBuffer.ReadRawInt();
            // var packet = ProtocolManager.Read(byteBuffer);
            // // queue it
            // receiveQueue.Enqueue(new Message(MessageType.Data, packet));
        }
    }
}