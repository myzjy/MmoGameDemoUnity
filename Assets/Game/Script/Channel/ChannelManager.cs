using System;
using Common.GameChannel.Implement;
using GameTools.Singletons;

namespace Common.GameChannel
{
    
    /// <summary>
    /// 通道管理程序
    /// </summary>
    public class ChannelManager:SingletonMMO<ChannelManager>
    {
        private BaseChannel channel = null;
        private Action onInitCompleted = null;
        private Action onActionSucceed = null;
        private Action onActionFailed = null;
        private Action<int> onActionProgressValueChange = null;
        public string channelName
        {
            get;
            protected set;
        }
        
        public string noticeVersion
        {
            get;
            set;
        }

        public string resVersion
        {
            get;
            set;
        }

        public string appVersion
        {
            get;
            set;
        }

        public string svnVersion
        {
            get;
            set;
        }
        public void Init(string channelName)
        {
            this.channelName = channelName;
            channel = CreateChannel(channelName);

            // AndroidSDKListener.Instance.Startup();
        }
        public bool IsInternalVersion()
        {
            return channel == null || channel.IsInternalChannel();
        }

        public BaseChannel CreateChannel(string channelName)
        {
            var platName = (ChannelType)Enum.Parse(typeof(ChannelType), channelName);
            switch ((platName))
            {
                case ChannelType.Test:
                    return new TestChannel();
                default:
                    return new TestChannel();
            }
            return null;
        }
        public void StartDownloadGame(string url, Action succeed = null, Action fail = null, Action<int> progress = null, string saveName = null)
        {
            onActionSucceed = succeed;
            onActionFailed = fail;
            onActionProgressValueChange = progress;
            channel.DownloadGame(url, saveName);
        }
        public string GetProductName()
        {
            return channel == null ? "xluaframework" : channel.GetProductName();
        }
        public void InstallGame(Action succeed, Action fail)
        {
            onActionSucceed = succeed;
            onActionFailed = fail;
            channel.InstallApk();
        }
        public bool IsGooglePlay()
        {
            return channel != null && channel.IsGooglePlay();
        }

        public override void Dispose()
        {
            
        }
    }
}