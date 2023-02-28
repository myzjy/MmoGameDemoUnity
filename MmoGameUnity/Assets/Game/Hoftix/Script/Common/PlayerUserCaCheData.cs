using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Common
{
    [Bean]
    public class PlayerUserCaCheData
    {
        /**
     * 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
     */
        public long DiamondNum;

        /**
     * 金币
     */
        public long goldNum;

        /**
     * 付费钻石 一般充值才有，付费钻石转换成普通钻石
     */
        public long PremiumDiamondNum;

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