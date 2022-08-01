namespace ZJYFrameWork.UISerializable.Manager
{
    public class GlobalDataManager : Singleton<GlobalDataManager>
    {
        public TimesManager time = new TimesManager();
    }
}