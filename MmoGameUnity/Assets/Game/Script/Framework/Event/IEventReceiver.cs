namespace ZJYFrameWork.Event
{
    public interface IEventReceiver
    {
        void Invoke(IEvent eve);
    }
}