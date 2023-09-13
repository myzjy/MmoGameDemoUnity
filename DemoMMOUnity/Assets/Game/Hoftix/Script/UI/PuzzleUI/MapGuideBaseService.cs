using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public abstract class MapGuideBaseService : MonoBehaviour
    {
        /// <summary>
        /// 地图type
        /// </summary>
        protected virtual int mapType { get; set; }

        protected virtual Action _action { get; set; }

        /// <summary>
        /// CanvasGroup
        /// </summary>
        public CanvasGroup BgTopCanvasGroup;

        /// <summary>
        /// 打开关卡地图
        /// </summary>
        public virtual void OnOpenMapGuide()
        {
            if (BgTopCanvasGroup != null)
            {
                BgTopCanvasGroup.alpha = 1;
                BgTopCanvasGroup.interactable = true;
                BgTopCanvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// 当返回章节选择触发
        /// </summary>
        public virtual void CloseMapGuide()
        {
            if (BgTopCanvasGroup != null)
            {
                BgTopCanvasGroup.alpha = 0;
                BgTopCanvasGroup.interactable = false;
                BgTopCanvasGroup.blocksRaycasts = false;
            }
        }
    }
}