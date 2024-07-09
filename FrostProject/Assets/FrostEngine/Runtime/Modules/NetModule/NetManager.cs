using System;

namespace FrostEngine
{
    internal sealed  class NetManager : ModuleImp, INetManager
    {
        private WebsocketClient WebsocketClient;
        private  Action<byte[]> receiveAction;

        public  void ReceiveString(byte[] bytes)
        {
        }

        internal override void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        public void Connect(string url)
        {
            WebsocketClient = new WebsocketClient(url);
            WebsocketClient.Start();
        }

        public void Close()
        {
            throw new System.NotImplementedException();
        }

        public void SendMessage(string bytes)
        {
            throw new System.NotImplementedException();
        }
        
        public  void ReceiveStringAction(Action<byte[]> receiveAction)
        {
            this.receiveAction = receiveAction;
        }

    }
}