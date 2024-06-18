#if !BESTHTTP_DISABLE_SOCKETIO

namespace BestHTTP.SocketIO
{
    public sealed class Error
    {
        private SocketIOErrors Code { get; set; }
        private string Message { get; set; }

        public Error(SocketIOErrors code, string msg)
        {
            this.Code = code;
            this.Message = msg;
        }

        public override string ToString()
        {
            return $"Code: {this.Code.ToString()} Message: \"{this.Message}\"";
        }
    }
}

#endif