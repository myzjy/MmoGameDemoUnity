namespace ZJYFrameWork.Module.PhysicalPower.Service
{
    public interface IPhysicalPowerService
    {
        /// <summary>
        /// 使用体力
        /// </summary>
        /// <param name="userNum"></param>
        void SendPhysicalPowerUserProps(int userNum);
    }
}