using DG.Tweening;
using GameUtil;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Module.Login.Service;
using ZJYFrameWork.Procedure.Scene;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Common;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public class LoginTapToStartView : UISerializableKeyObject
    {
        public Button LoginStartMaxButton = null;
        public Button LoginStartButton = null;
        public UILoginView LoginView = null;

        /// <summary>
        /// 赋值 构建 传递过来LoginView，便于控制
        /// </summary>
        /// <param name="loginView"></param>
        public void Build(UILoginView loginView)
        {
            LoginView = loginView;
            LoginStartButton = GetObjType<Button>("LoginStartButton");
            LoginStartMaxButton = GetObjType<Button>("LoginStartMaxButton");
            if (LoginStartButton == null)
            {
#if DEVELOP_BUILD
                Debug.LogError($"请检查[{name}]物体配置下面是否有[LoginStartButton]组件");
#endif
            }
            else
            {
                LoginStartButton.SetListener(StartGame);
            }

            if (LoginStartMaxButton == null)
            {
#if DEVELOP_BUILD
                Debug.LogError($"请检查{name}物体配置下面是否有[LoginStartMaxButton]组件");
#endif
            }
            else
            {
                LoginStartMaxButton.SetListener(() => { SpringContext.GetBean<LoginUIController>().OnHide(); });
            }
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            var buttonTapToStartCanvasGroup = GetComponent<CanvasGroup>();
            buttonTapToStartCanvasGroup.DOFade(0f, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                buttonTapToStartCanvasGroup.interactable = false;
                buttonTapToStartCanvasGroup.ignoreParentGroups = false;
                buttonTapToStartCanvasGroup.blocksRaycasts = false;
                buttonTapToStartCanvasGroup.interactable = false;
            }).Play();
            gameObject.SetActive(false);
        }

        public void Show()
        {
            // var sequence = DOTween.Sequence();
            // sequence.AppendCallback(() => { CommonController.Instance.loadingRotate.OnShow(); });
            // sequence.AppendInterval(1.5f);
            // sequence.AppendCallback(() => { CommonController.Instance.loadingRotate.OnClose(); });
            // sequence.AppendInterval(0.5f);
            //
            // sequence.AppendCallback(() =>
            // {
            gameObject.SetActive(true);

            CommonController.Instance.loadingRotate.OnClose();
            var buttonTapToStartCanvasGroup = GetComponent<CanvasGroup>();
            buttonTapToStartCanvasGroup.DOFade(1f, 1.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                buttonTapToStartCanvasGroup.interactable = true;
                buttonTapToStartCanvasGroup.ignoreParentGroups = true;
                buttonTapToStartCanvasGroup.blocksRaycasts = true;
                buttonTapToStartCanvasGroup.interactable = true;
            }).Play();
            // });
        }

        protected virtual void StartGame()
        {
            SpringContext.GetBean<ILoginService>().LoginTapToStart();
        }

        public void LoginStartGame()
        {
            var buttonTapToStartCanvasGroup = GetComponentInChildren<CanvasGroup>();
            buttonTapToStartCanvasGroup.DOKill();
            buttonTapToStartCanvasGroup.DOFade(1f, 0.2f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    SpringContext.GetBean<LoginUIController>().OnHide();
                    //跳转场景 
                    SpringContext.GetBean<ProcedureChangeScene>().ChangeScene(SceneEnum.GameMain, "GameMain");
                });
        }
    }
}