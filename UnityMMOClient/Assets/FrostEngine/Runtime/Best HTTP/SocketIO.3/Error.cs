#if !BESTHTTP_DISABLE_SOCKETIO

namespace BestHTTP.SocketIO3
{
    public class Error
    {
        private readonly string _message;

        public Error() { }

        public Error(string msg)
        {
            this._message = msg;
        }

        public override string ToString()
        {
            return this._message;
        }
    }
}

#endif
