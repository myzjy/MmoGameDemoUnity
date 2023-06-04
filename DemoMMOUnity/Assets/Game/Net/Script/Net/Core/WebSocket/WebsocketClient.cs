using System.Collections.Generic;
using BestHTTP.WebSocket;
using Newtonsoft.Json;
using ZJYFrameWork.Event;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.Dispatcher;
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
            _webSocketBridge.websocketClient = this;
            _webSocketBridge.WebSocketClose();
            _webSocketBridge.WebSocketConnect(url);
        }

        public override bool Connected()
        {
            throw new System.NotImplementedException();
        }

        public override void Close()
        {
            _webSocketBridge.WebSocketClose();
        }

        public override string ToConnectUrl()
        {
            return url;
        }

        protected override void SendMessagesBlocking(byte[] messages, int offset, int size)
        {
            throw new System.NotImplementedException();
        }

        protected override bool ReadMessageBlocking(out byte[] content)
        {
            throw new System.NotImplementedException();
        }

        public override bool Send(byte[] data)
        {
            _webSocketBridge.WebSocketSend(data);
            return true;
        }

        /// <summary>
        /// 打开回调
        /// </summary>
        internal void HandleOnOpen(WebSocket ws)
        {
            Debug.Log("成功打开");
            Debug.Log("Connected server [{}]", ToConnectUrl());

            EventBus.AsyncSubmit(NetOpenEvent.ValueOf());
        }

        internal void HandleOnMessage(string message)
        {
            var byteBuffer = ByteBuffer.ValueOf();
            var byteString = StringUtils.Bytes(message);
            byteBuffer.WriteBytes(byteString);

            byteBuffer.ReadRawInt();
            PacketDispatcher.ReceiveString(byteString);

            var packet = ProtocolManager.Read(byteBuffer);
            if (packet != null)
            {
                PacketDispatcher.Receive(packet);
            }

            // // queue it
            // receiveQueue.Enqueue(new Message(MessageType.Data, packet));
        }

        internal void HandleOnMessage(byte[] content)
        {
            var byteBuffer = ByteBuffer.ValueOf();
            byteBuffer.WriteBytes(content);

            byteBuffer.ReadRawInt();
            PacketDispatcher.ReceiveString(content);
            var packet = ProtocolManager.Read(byteBuffer);
            if (packet != null)
            {
                PacketDispatcher.Receive(packet);
            }
            // // queue it
            // receiveQueue.Enqueue(new Message(MessageType.Data, packet));
        }

        internal void HandleOnError(string reason)
        {
            Debug.LogError(reason);
            EventBus.AsyncSubmit(NetErrorEvent.ValueOf());
        }

        internal void HandleOnClose(ushort code, string message)
        {
            Debug.LogError(message);
        }
    }
}