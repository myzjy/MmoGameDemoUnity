namespace ZJYFrameWork.UISerializable
{
    public class UINotification
    {
        private string eventName;

        public string GetEventName => eventName;


        private object eventBody = null;

        public object GetEventBody => eventBody;

        public T GetParms<T>()
        {
            return (T) eventBody;
        }

        public UINotification(string name, object body)
        {
            eventName = name;
            eventBody = body;
        }

        public void SetNotification(string name, object body)
        {
            eventName = name;
            eventBody = body;
        }
    }
}