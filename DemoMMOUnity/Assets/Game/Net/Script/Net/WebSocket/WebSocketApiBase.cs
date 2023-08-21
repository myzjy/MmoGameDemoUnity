using System;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.Websocket
{
    public abstract class WebSocketApiBase<wbRequest,wbResponse,wbError>
    where wbRequest:Model,IPacket,IReference
    where wbResponse:Model,IPacket,IReference
    {
        public bool isDebug;
        public Action onBeforeSend;
        public Action onComplete;
        public wbRequest Param { get; protected set; }
        public Action<wbResponse> onSuccess;
    }
}