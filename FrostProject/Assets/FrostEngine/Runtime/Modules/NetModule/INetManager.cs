namespace FrostEngine
{
    public interface INetManager
    {
        void Connect(string url);

        void Close();
        
        void SendMessage(string bytes);
    }
}