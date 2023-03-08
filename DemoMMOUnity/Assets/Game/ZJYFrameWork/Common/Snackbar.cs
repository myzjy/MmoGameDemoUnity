using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.UI.UIModel;

namespace ZJYFrameWork.UISerializable.Common
{
    public class Snackbar : MonoBehaviour
    {
        // #region 竖屏
        //
        // public GameObject Root;
        // public CanvasGroup serverErrorCanvasGroup;
        // public Text serverError;
        // public CanvasGroup errorCanvasGroup;
        // public Text error;
        //
        // public CanvasGroup infoCanvasGroup;
        // public Text info;
        //
        // public CanvasGroup serverInfoCanvasGroup;
        // public Text serverInfo;
        //
        // #endregion

        #region 横屏 LandscapeLeft

        public GameObject RootLandscapeLeft;

        public CanvasGroup serverErrorLandscapeLeftCanvasGroup;
        public Text serverLandscapeLeftError;
        public UIDataLoading UIDataLoading;
        public Dialog OverlayDialog;

        #endregion

        #region 进度条显示

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="downText"></param>
        /// <param name="nowDownNums"></param>
        /// <param name="maxDownNums"></param>
        public void OpenUIDataLoadingPanel(string downText, float nowDownNums, float maxDownNums, string tips = "")
        {
            if (UIDataLoading.GetSelfObjCanvasGroup.alpha < 1)
            {
                UIDataLoading.OnOpen();
            }

            UIDataLoading.SetShowCount(nowDownNums, maxDownNums, downText);
        }

        /// <summary>
        /// 转换场景的时候调用显示面板
        /// </summary>
        /// <param name="nowDownNums">当前进度</param>
        /// <param name="maxDownNums">最大进度</param>
        public void OpenUIDataScenePanel(float nowDownNums, float maxDownNums)
        {
            if (UIDataLoading.GetSelfObjCanvasGroup.alpha < 1)
            {
                UIDataLoading.OnOpen();
            }

            UIDataLoading.SetSceneProgress(nowDownNums, maxDownNums);
        }

        #endregion

        public void SeverError(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            RootLandscapeLeft.SetActive(true);
            serverLandscapeLeftError.text = message;
            serverErrorLandscapeLeftCanvasGroup.gameObject.SetActive(true);
            serverErrorLandscapeLeftCanvasGroup.alpha = 0;
            serverErrorLandscapeLeftCanvasGroup.DOFade(1f, 0.8f).SetEase(Ease.Linear).SetLoops(1, LoopType.Yoyo);
            StartCoroutine(HideServerError(message));
        }

        private IEnumerator HideServerError(string message)
        {
            yield return new WaitForSeconds(3f);

            if (message.Equals(serverLandscapeLeftError.text))
            {
                serverErrorLandscapeLeftCanvasGroup.DOFade(0f, 1.2f)
                    .SetEase(Ease.Linear)
                    .SetLoops(1, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        if (message.Equals(serverLandscapeLeftError.text))
                        {
                            serverLandscapeLeftError.text = null;
                            serverErrorLandscapeLeftCanvasGroup.gameObject.SetActive(false);
                        }
                    });
            }
        }

        #region 提示框

        ///<summary>
        ///通知窗打开
        ///</summary>
        public void OpenCommonUIPanel(Dialog.ButtonType buttonType, string titleText, string message,
            Action<Dialog.Result> onClick, string YesButtonText, string NoButtonText, System.Action onOpen = null,
            System.Action onClose = null)
        {
            OverlayDialog.Open(buttonType, titleText, message, YesButtonText, NoButtonText, onClick, onOpen,
                onClose);
        }

        #endregion
    }
}