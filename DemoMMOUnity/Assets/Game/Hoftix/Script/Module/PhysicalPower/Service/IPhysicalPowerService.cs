namespace ZJYFrameWork.Module.PhysicalPower.Service
{
    public interface IPhysicalPowerService
    {
        /// <summary>
        /// 使用体力
        /// </summary>
        /// <param name="userNum"></param>
        void SendPhysicalPowerUserProps(int userNum);

        /// <summary>
        /// 恢复体力 每秒
        /// </summary>
        /// <param name="nowTime">时间</param>
        void SendPhysicalPowerSecondsRequest(long nowTime);

        /// <summary>
        /// 请求服务器体力信息
        /// </summary>
        void SendPhysicalPowerRequest();
    }
}