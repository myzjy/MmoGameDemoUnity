namespace Common.GameChannel.Implement
{
    /// <summary>
    /// 测试渠道
    /// </summary>
    public class TestChannel: BaseChannel
    {
        public override void Init()
        {
            
        }

        public override void DownloadGame(params object[] paramList)
        {
            string url = paramList[0] as string;
            string saveName = paramList[1] as string;
        }

        public override string GetBundleID()
        {
            return "com.chivas.xluaframework";
        }

        public override void InstallApk()
        {
        }

        public override string GetProductName()
        {
            return "xluaframework";
        }

        public override void Login()
        {
        }

        public override void Logout()
        {
        }

        public override void Pay(params object[] paramList)
        {
        }
    }
}