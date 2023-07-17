using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Hotfix.Common
{
    /// <summary>
    /// 体力
    /// </summary>
    [Bean]
    public class PhysicalPowerCacheData
    {
        /**
    * 返回 使用体力 所扣除 当前体力
    */
        private int nowPhysicalPower;

        /**
         * 一点体力增长剩余时间
         * <p> 注意这里不是时间戳赋值</p>
         */
        private int residueTime;

        /**
         * 当前体力实时时间 会跟着剩余时间一起变化
         */
        private long residueNowTime;

        /**
         * 最大体力 用于限制 这个值会随着 等级增长
         */
        private int maximumStrength;


        /**
         * 我恢复到最大体力的结束时间
         * <p>这里不是时间戳<p/>
         */
        private int maximusResidueEndTime;


        public int getNowPhysicalPower()
        {
            return nowPhysicalPower;
        }

        public void setNowPhysicalPower(int nowPhysicalPower)
        {
            this.nowPhysicalPower = nowPhysicalPower;
        }

        public int getResidueTime()
        {
            return residueTime;
        }

        public void setResidueTime(int residueTime)
        {
            this.residueTime = residueTime;
        }

        public long getResidueNowTime()
        {
            return residueNowTime;
        }

        public void setResidueNowTime(long residueNowTime)
        {
            this.residueNowTime = residueNowTime;
        }

        public int getMaximumStrength()
        {
            return maximumStrength;
        }

        public void setMaximumStrength(int maximumStrength)
        {
            this.maximumStrength = maximumStrength;
        }

        public int getMaximusResidueEndTime()
        {
            return maximusResidueEndTime;
        }

        public void setMaximusResidueEndTime(int maximusResidueEndTime)
        {
            this.maximusResidueEndTime = maximusResidueEndTime;
        }

        public void SetPhysicalPowerSecondsResponse(PhysicalPowerSecondsResponse response)
        {
            setResidueTime(residueTime: response.residueTime);
            setMaximumStrength(response.maximumStrength);
            setNowPhysicalPower(response.nowPhysicalPower);
            setMaximusResidueEndTime(response.maximusResidueEndTime);
            setResidueNowTime(residueNowTime: response.residueNowTime);
        }
        public void SetPhysicalPowerUserPropsResponse(PhysicalPowerUserPropsResponse response)
        {
            setResidueTime(residueTime: response.residueTime);
            setMaximumStrength(response.maximumStrength);
            setNowPhysicalPower(response.nowPhysicalPower);
            setMaximusResidueEndTime(response.maximusResidueEndTime);
            setResidueNowTime(residueNowTime: response.residueNowTime);
        }
        
    }
}