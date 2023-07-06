using System;
using UnityEngine;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Event;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Net.Core.Websocket;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Spring.Core;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.Net
{
    [Bean]
    public class NetManager : AbstractManager, INetManager
    {
        /// <summary>
        /// 网络客户端
        /// </summary>
        private AbstractClient netClient;

        /// <summary>
        /// 
        /// </summary>
        public Action<string> LuaConnectAction
        {
            get => luaConnectAction;
            set => luaConnectAction = value;
        }

        private Action<string> luaConnectAction = null;

        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="url"></param>
        public void Connect(string url)
        {
            Close();
            LuaConnectAction.Invoke($"开始链接服务器[url:{url}][Platform:{Application.platform}]");
            Debug.Log($"开始链接服务器[url:{url}][Platform:{Application.platform}]");
            netClient = new WebsocketClient(url);
            netClient.Start();
        }

        /// <summary>
        /// 关闭socket
        /// </summary>
        public void Close()
        {
            if (netClient != null)
            {
                netClient.Close();
                netClient = null;
            }
        }

        public void Send(IPacket packet)
        {
            if (netClient == null)
            {
                throw new NullReferenceException("网络客户端未开启");
            }

            ByteBuffer byteBuffer = null;
            try
            {
                byteBuffer = ByteBuffer.ValueOf();
                ProtocolManager.Write(byteBuffer, packet);
                var sendSuccess = netClient.Send(byteBuffer.ToBytes());
                if (!sendSuccess)
                {
                    Close();
                    // EventBus.AsyncSubmit(NetErrorEvent.ValueOf());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (byteBuffer != null)
                {
                    byteBuffer.Clear();
                }
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
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 轮询
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (netClient == null)
            {
                return;
            }

            //     while (netClient.GetNextMessage(out var message))
            //     {
            //         switch (message.messageType)
            //         {
            //             case MessageType.Connected:
            //                 Debug.Log("Connected server [{}]", netClient.ToConnectUrl());
            //
            //                 break;
            //             case MessageType.Data:
            //                 PacketDispatcher.Receive(message.packet);
            //
            //                 break;
            //             case MessageType.Disconnected:
            //                 Debug.Log("Disconnected");
            //
            //                 break;
            //             default:
            //                 throw new ArgumentOutOfRangeException();
            //         }
            //     }
        }

        public override void Shutdown()
        {
            Close();
        }
    }
}