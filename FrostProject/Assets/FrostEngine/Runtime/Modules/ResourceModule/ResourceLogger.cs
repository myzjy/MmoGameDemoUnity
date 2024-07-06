namespace FrostEngine
{
    internal class ResourceLogger : YooAsset.ILogger
    {
        public void Log(string message)
        {
            Debug.Info(message);
        }

        public void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public void Error(string message)
        {
            Debug.LogError(message);
        }

        public void Exception(System.Exception exception)
        {
            Debug.Fatal(exception.Message);
        }
    }
}