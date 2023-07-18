using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    /// <summary>
    /// 体力 Response
    /// </summary>
    public class PhysicalPowerResponse : Model, IPacket, IReference
    {
        /// <summary>
        /// 当前体力
        /// </summary>
        public int nowPhysicalPower { get; set; }

        /**
         * 一点体力增长剩余时间
         * <p> 注意这里不是时间戳赋值</p>
         */
        public int residueTime { get; set; }
        /**
         * 最大体力 用于限制 这个值会随着 等级增长
         */
        public int maximumStrength { get; set; }
        /**
         * 我恢复到最大体力的结束时间
         * <p>这里不是时间戳</p>
         */
        public int maximusResidueEndTime { get; set; }

        /**
         * 当前体力实时时间 会跟着剩余时间一起变化
         * */
        public long residueNowTime { get; set; }

        public short ProtocolId()
        {
            return 1024;
        }

        public void Clear()
        {
            nowPhysicalPower = 0;
            residueTime = 0;
            maximumStrength = 0;
            maximusResidueEndTime = 0;
            residueNowTime = 0;
        }

        public static PhysicalPowerSecondsResponse ValueOf()
        {
            var packet = ReferenceCache.Acquire<PhysicalPowerSecondsResponse>();
            packet.Clear();
            return packet;
        }

        public static PhysicalPowerSecondsResponse ValueOf(int nowPhysicalPower, int residueTime, int maximumStrength,
            int maximusResidueEndTime, int residueNowTime)
        {
            var packet = ReferenceCache.Acquire<PhysicalPowerSecondsResponse>();
            packet.Clear();
            packet.nowPhysicalPower = nowPhysicalPower;
            packet.residueTime = residueTime;
            packet.maximumStrength = maximumStrength;
            packet.maximusResidueEndTime = maximusResidueEndTime;
            packet.residueNowTime = residueNowTime;
            return packet;
        }
    }
}