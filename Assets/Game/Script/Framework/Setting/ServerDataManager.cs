using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Setting
{
    [Bean]
    public sealed class ServerDataManager
    {
        
        [AfterPostConstruct]
        public void Init()
        {
            
        }
    }
}