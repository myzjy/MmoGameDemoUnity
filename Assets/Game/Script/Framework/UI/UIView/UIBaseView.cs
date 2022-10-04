using System;
using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public abstract class UIBaseView<PanelView> where PanelView : UIViewInterface, new()
    {
        /// <summary>
        /// UI实例化时调用 OnInit
        /// </summary>
        public abstract void OnInit();

        /// <summary>
        /// 当物体打开时调用
        /// </summary>
        public abstract void OnShow();

        /// <summary>
        /// 当Canvas CanvasGroup GraphicRaycaster 设置为不激活时调用
        /// </summary>
        public virtual void OnHide()
        {
            UnityEngine.Debug.Log("隐藏");
            SelfUIView.OnClose();
        }

        [Obsolete("请调用OnHide方法")]
        public virtual void OnClickDestroy()
        {
        }

        public void ReUse()
        {
            SelfUIView.OnShow();
            OnShow();
        }

        /// <summary>
        /// 物体是否隐藏
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (SelfUIView != null)
                {
                    if (SelfUIView.GetSelfObjCanvasGroup)
                    {
                        var isActive = SelfUIView.GetSelfObjCanvasGroup.alpha > 0;
                        return isActive;
                    }
                    else
                    {
#if UNITY_EDITOR
                        // Debug.LogError(
                        //     $"UIView组件 缺少{typeof(Canvas)},缺少{typeof(GraphicRaycaster)},缺少{typeof(CanvasGroup)},");
#endif
                    }

                    return false;
                }

                return false;
            }
        }

        /// <summary>
        /// 每个预制体都必须需要的UIview
        /// </summary>
        private UIView SelfUIView;

        public UIView GetSelfUIView => SelfUIView;

        /// <summary>
        /// 为ui显示面板 所创建脚本存放所有要使用的脚本 UI组件全部通过这里去调度
        /// </summary>
        public PanelView viewPanel;


        public void SetUIView(UIView view)
        {
            SelfUIView = view;
            viewPanel = new PanelView();
            viewPanel.Init(SelfUIView);
        }
    }
}