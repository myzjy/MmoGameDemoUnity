using System;

namespace FrostEngine
{
    internal sealed  class NetManager : ModuleImp, INetManager
    {
        /// <summary>
        /// 网络客户端
        /// </summary>
        private AbstractClient netClient;
        private  Action<byte[]> receiveAction;

        public  void ReceiveString(byte[] bytes)
        {
            receiveAction.Invoke(bytes);
        }

        internal override void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        public bool Connect(string url)
        {
            netClient = new WebsocketClient(url);
            netClient.Start();
            return netClient.Connected();
        }

        public bool IsConnect()
        {
            return netClient.Connected();
        }

        public void Close()
        {
            if (netClient != null)
            {
                netClient.Close();
                netClient = null;
            }
        }

        public void SendMessage(string bytes)
        {
            try
            {
                ByteBuffer byteBuffer = ByteBuffer.ValueOf();
                byteBuffer.WriteString(bytes);
                var sendSuccess = netClient.Send(byteBuffer.ToBytes());
                if (!sendSuccess)
                {
                    Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }
        
        public  void ReceiveStringAction(Action<byte[]> receiveAction)
        {
            this.receiveAction = receiveAction;
        }

    }
}