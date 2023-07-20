using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;

namespace ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.UserInfo
{
    /// <summary>
    ///  GameMain UserInfo 信息
    /// Protocol id  :1032
    /// </summary>
    public class GameMainUserToInfoResponse : Model, IPacket, IReference
    {
        public short ProtocolId()
        {
            return 1032;
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        /**
         * 当前经验
         */
        private int nowExp;

        /**
         * 最大经验
         */
        private int maxExp;

        /**
         * 当前等级
         */
        private int nowLv;

        /**
         * 最大等级
         */
        private int maxLv;

        /**
         * 金币
         */
        private long goldCoinNum;

        /**
         * 钻石
         */
        private long diamondsNum;

        /**
         * 付费钻石
         */
        private long paidDiamondsNum;

        public int getNowExp()
        {
            return nowExp;
        }

        public void setNowExp(int nowExp)
        {
            this.nowExp = nowExp;
        }

        public int getMaxExp()
        {
            return maxExp;
        }

        public void setMaxExp(int maxExp)
        {
            this.maxExp = maxExp;
        }

        public int getNowLv()
        {
            return nowLv;
        }

        public void setNowLv(int nowLv)
        {
            this.nowLv = nowLv;
        }

        public int getMaxLv()
        {
            return maxLv;
        }

        public void setMaxLv(int maxLv)
        {
            this.maxLv = maxLv;
        }

        public long getGoldCoinNum()
        {
            return goldCoinNum;
        }

        public void setGoldCoinNum(long goldCoinNum)
        {
            this.goldCoinNum = goldCoinNum;
        }

        public long getDiamondsNum()
        {
            return diamondsNum;
        }

        public void setDiamondsNum(long diamondsNum)
        {
            this.diamondsNum = diamondsNum;
        }

        public long getPaidDiamondsNum()
        {
            return paidDiamondsNum;
        }

        public void setPaidDiamondsNum(long paidDiamondsNum)
        {
            this.paidDiamondsNum = paidDiamondsNum;
        }

        /**
         *  返回结构
         * @param nowLv 当前登录
         * @param maxLv 最大等级
         * @param nowExp 当前经验
         * @param maxExp 最大经验
         * @param goldCoinNum 当前金币
         * @param diamondsNum 钻石
         * @param paidDiamondsNum 付费钻石
         * @return 返回 GameMainUserInfoToResponse
         */
        public static GameMainUserToInfoResponse ValueOf(int nowLv, int maxLv, int nowExp, int maxExp, long goldCoinNum,
            long diamondsNum, long paidDiamondsNum)
        {
            GameMainUserToInfoResponse data = new GameMainUserToInfoResponse();
            data.setNowLv(nowLv);
            data.setMaxLv(maxLv);
            data.setNowExp(nowExp);
            data.setMaxExp(maxExp);
            data.setGoldCoinNum(goldCoinNum);
            data.setDiamondsNum(diamondsNum);
            data.setPaidDiamondsNum(paidDiamondsNum);
            return data;
        }
    }

    public class GameMainUserToInfoResponseRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1032;
        }

        public void Write(ByteBuffer buffer, IPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            var data = (GameMainUserToInfoResponse)packet;
            var serverMessage = new ServerMessageWrite(data.ProtocolId(), data);
            var jsonStr = JsonConvert.SerializeObject(serverMessage);
            buffer.WriteString(jsonStr);
        }

        public IPacket Read(ByteBuffer buffer, string json = "")
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return null;
        }
    }
}