using UnityEngine;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UI.UIModel;
using ZJYFrameWork.UISerializable.Common;

namespace ZJYFrameWork.UISerializable
{
    public class LoadUIController : MonoBehaviour
    {
        public LoadingUIView UIView;

        public void Build(LoadingUIView loadingUIView)
        {
            //引用指针 
            UIView = loadingUIView;
            SpringContext.RegisterBean(this);
        }

        /// <summary>
        /// 显示
        /// </summary>
        public void OnShow()
        {
            if (!OnErrorTips())
                return;
            if (!UIView.IsActive)
            {
                //物体隐藏了
            }

            UIView.OnShow();
        }

        private bool OnErrorTips()
        {
            //这个判断应该不会进入，除非代码放错地方调用出错 
            //一般只会有调用顺序出现问题才会出现UIView为空
            if (UIView != null) return true;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"当前的{typeof(LoadingUIView)}所需的UI界面并未生成，调用顺序有问题");
            //只有编辑器下提示，让代码人员注意检查
#if UNITY_EDITOR
            CommonController.Instance.snackbar.OpenCommonUIPanel(Dialog.ButtonType.YesNo, "", "生成顺序有问题，请检查代码",
                res => { }, "确定", "取消");
#endif

#endif
            return false;
        }

        public void OnHide()
        {
            if (!OnErrorTips())
                return;
            UIView.OnHide();
        }

        public void SetNowProgressNum(float nums)
        {
            if (!OnErrorTips())
                return;
            UIView.SetNowProgressNum(nums);
        }

        public void RefreshProgressLoginLater(float progressNum)
        {
            if (!OnErrorTips())
                return;
            UIView.RefreshProgressLoginLater(progressNum);
        }
    }
}