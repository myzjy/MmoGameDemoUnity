﻿using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Event;

namespace ZJYFrameWork.Net.Core.Model
{
    public class LoadSceneSuccessEvent : IEvent, IReference
    {
        /// <summary>
        /// 获取加载持续时间。
        /// </summary>
        public float duration;

        /// <summary>
        /// 获取场景资源名称。
        /// </summary>
        public string sceneAssetName;

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object userData;

        public void Clear()
        {
            sceneAssetName = null;
            duration = 0f;
            userData = null;
        }

        public static LoadSceneSuccessEvent ValueOf(string sceneAssetName, float duration, object userData)
        {
            var eve = ReferenceCache.Acquire<LoadSceneSuccessEvent>();
            eve.sceneAssetName = sceneAssetName;
            eve.duration = duration;
            eve.userData = userData;
            return eve;
        }
    }
}