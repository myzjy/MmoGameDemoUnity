using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Common
{
    [Bean]
    public class PlayerUserCaCheData
    {
        /// <summary>
        /// 玩家Uid
        /// </summary>
        public long Uid;

        /// <summary>
        /// 玩家的名字
        /// </summary>
        public string userName;
    }
}