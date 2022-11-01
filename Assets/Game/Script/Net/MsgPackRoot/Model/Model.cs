﻿using System;
using Newtonsoft.Json;
using UnityEngine;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net
{
    public abstract class Model
    {
        /// <summary>
        /// JSONにシリアライズ
        /// </summary>
        public abstract string ToJson(bool isPretty = false);

        /// <summary>
        /// 文字列に変換
        /// </summary>
        public override string ToString()
        {
            return ToJson();
        }

        /// <summary>
        /// MsgPackのバイナリを変数に流し込む
        /// </summary>
        public virtual void Unpack(byte[] bytes)
        {
            throw new NotImplementedException("Unpack(byte[] bytes) is not Implemented for class " +
                                              GetType().FullName);
        }

        /// <summary>
        /// MsgPackフォーマットに変換
        /// </summary>
        public virtual byte[] Pack()
        {
            var str = JsonConvert.SerializeObject(this);
            Debug.Log(str);
            var byteBuff = ByteBuffer.ValueOf();
            byteBuff.WriteString(str);
            return byteBuff.ToBytes();
            throw new NotImplementedException("Pack is not Implemented for class " + GetType().FullName);
        }

        /// <summary>
        /// クエリパラメータを生成
        /// </summary>
        public virtual string BuildQuery()
        {
            throw new NotImplementedException("BuildQuery is not Implemented for class " + GetType().FullName);
        }

        public virtual void Update<T>(T model) where T : Model
        {
            throw new NotImplementedException("Update(T model) is not implemented for class " + GetType().FullName);
        }
    }
}