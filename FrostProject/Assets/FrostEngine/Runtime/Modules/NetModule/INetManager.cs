namespace FrostEngine
{
    public interface INetManager
    {
        bool Connect(string url);

        void Close();
        
        void SendMessage(string bytes);
        bool IsConnect();
    }
}