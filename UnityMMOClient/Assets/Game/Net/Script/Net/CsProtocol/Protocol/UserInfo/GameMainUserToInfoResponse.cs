using System.Collections.Generic;
using FrostEngine;
using Newtonsoft.Json;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Net.Core;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.CsProtocol.Protocol.UserInfo
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
            _nowExp = 0;
            _maxExp = 0;
            _diamondsNum = 0;
            _maxLv = 0;
            _goldCoinNum = 0;
            _nowLv = 0;
            _paidDiamondsNum = 0;
        }

        /**
         * 当前经验
         */
        private int _nowExp;

        /**
         * 最大经验
         */
        private int _maxExp;

        /**
         * 当前等级
         */
        private int _nowLv;

        /**
         * 最大等级
         */
        private int _maxLv;

        /**
         * 金币
         */
        private long _goldCoinNum;

        /**
         * 钻石
         */
        private long _diamondsNum;

        /**
         * 付费钻石
         */
        private long _paidDiamondsNum;

        public int GetNowExp()
        {
            return _nowExp;
        }

        public void SetNowExp(int nowExp)
        {
            this._nowExp = nowExp;
        }

        public int GetMaxExp()
        {
            return _maxExp;
        }

        public void SetMaxExp(int maxExp)
        {
            this._maxExp = maxExp;
        }

        public int GetNowLv()
        {
            return _nowLv;
        }

        public void SetNowLv(int nowLv)
        {
            this._nowLv = nowLv;
        }

        public int GetMaxLv()
        {
            return _maxLv;
        }

        public void SetMaxLv(int maxLv)
        {
            this._maxLv = maxLv;
        }

        public long GetGoldCoinNum()
        {
            return _goldCoinNum;
        }

        public void SetGoldCoinNum(long goldCoinNum)
        {
            this._goldCoinNum = goldCoinNum;
        }

        public long GetDiamondsNum()
        {
            return _diamondsNum;
        }

        public void SetDiamondsNum(long diamondsNum)
        {
            this._diamondsNum = diamondsNum;
        }

        public long GetPaidDiamondsNum()
        {
            return _paidDiamondsNum;
        }

        public void SetPaidDiamondsNum(long paidDiamondsNum)
        {
            this._paidDiamondsNum = paidDiamondsNum;
        }

        public static GameMainUserToInfoResponse ValueOf()
        {
            var data = ReferenceCache.Acquire<GameMainUserToInfoResponse>();
            data.Clear();
            return data;
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
            data.SetNowLv(nowLv);
            data.SetMaxLv(maxLv);
            data.SetNowExp(nowExp);
            data.SetMaxExp(maxExp);
            data.SetGoldCoinNum(goldCoinNum);
            data.SetDiamondsNum(diamondsNum);
            data.SetPaidDiamondsNum(paidDiamondsNum);
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

        public IPacket Read(ByteBuffer buffer)
        {
            var json = StringUtils.BytesToString(buffer.ToBytes());
            var packet = ReferenceCache.Acquire<GameMainUserToInfoResponse>();
            packet.Clear();
            if (string.IsNullOrEmpty(json))
            {
                return packet;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
            if (dict == null)
            {
#if UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG)
                Debug.LogError("消息解析错误，请检查");
#endif
                return packet;
            }

            foreach (var (key, value) in dict)
            {
                var keyStr = key.ToString();
                var valueStr = value.ToString();
                switch (keyStr)
                {
                    case "nowLv":
                    {
                        var nowLv = int.Parse(valueStr);
                        packet.SetNowLv(nowLv);
                    }
                        break;
                    case "diamondsNum":
                    {
                        var diamondsNum = int.Parse(valueStr);
                        packet.SetDiamondsNum(diamondsNum);
                    }
                        break;
                    case "maxExp":
                    {
                        var maxExp = int.Parse(valueStr);
                        packet.SetMaxExp(maxExp);
                    }
                        break;
                    case "nowExp":
                    {
                        var nowExp = int.Parse(valueStr);
                        packet.SetNowExp(nowExp);
                    }
                        break;
                    case "maxLv":
                    {
                        var maxLv = int.Parse(valueStr);
                        packet.SetMaxLv(maxLv);
                    }
                        break;
                    case "goldCoinNum":
                    {
                        var goldCoinNum = long.Parse(valueStr);
                        packet.SetGoldCoinNum(goldCoinNum);
                    }
                        break;
                    case "paidDiamondsNum":
                    {
                        var paidDiamondsNum = long.Parse(valueStr);
                        packet.SetPaidDiamondsNum(paidDiamondsNum);
                    }
                        break;
                }
            }

            return null;
        }
    }
}