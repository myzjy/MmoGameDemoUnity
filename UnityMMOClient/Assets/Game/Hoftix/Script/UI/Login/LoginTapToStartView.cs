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
    public class LoginTapToStartView 
    {
        public Button LoginStartMaxButton = null;
        public Button LoginStartButton = null;
        public CanvasGroup SteamLoginCanvasGroup = null;

        /// <summary>
        /// 赋值 构建 传递过来LoginView，便于控制
        /// </summary>
        /// <param name="loginView"></param>
        public void Build(UISerializableKeyObject loginView)
        {
            LoginStartButton = loginView.GetObjType<Button>("LoginStartButton");
            LoginStartMaxButton = loginView.GetObjType<Button>("LoginStartMaxButton");
            SteamLoginCanvasGroup = loginView.GetObjType<CanvasGroup>("LoginStart_CanvasGroup");
            if (LoginStartButton == null)
            {
#if DEVELOP_BUILD
                Debug.LogError($"请检查[{loginView.name}]物体配置下面是否有[LoginStartButton]组件");
#endif
            }
            else
            {
                LoginStartButton.SetListener(StartGame);
            }

            if (LoginStartMaxButton == null)
            {
#if DEVELOP_BUILD
                Debug.LogError($"请检查{loginView.name}物体配置下面是否有[LoginStartMaxButton]组件");
#endif
            }
            else
            {
                LoginStartMaxButton.SetListener(() => { SpringContext.GetBean<LoginUIController>().OnHide(); });
            }
        }

        public void Hide()
        {
            if (SteamLoginCanvasGroup.alpha < 1)
            {
                return;
            }

            SteamLoginCanvasGroup.DOFade(0f, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                SteamLoginCanvasGroup.interactable = false;
                SteamLoginCanvasGroup.ignoreParentGroups = false;
                SteamLoginCanvasGroup.blocksRaycasts = false;
                SteamLoginCanvasGroup.interactable = false;
            }).Play();
        }

        public void Show()
        {
            CommonController.Instance.loadingRotate.OnClose();
            SteamLoginCanvasGroup.DOFade(1f, 1.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                SteamLoginCanvasGroup.interactable = true;
                SteamLoginCanvasGroup.ignoreParentGroups = true;
                SteamLoginCanvasGroup.blocksRaycasts = true;
                SteamLoginCanvasGroup.interactable = true;
            }).Play();
        }

        protected virtual void StartGame()
        {
            SpringContext.GetBean<ILoginService>().LoginTapToStart();
        }

        public void LoginStartGame()
        {
            SteamLoginCanvasGroup.DOKill();
            SteamLoginCanvasGroup.DOFade(1f, 0.2f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    SpringContext.GetBean<LoginUIController>().OnHide();
                    //跳转场景 
                    SpringContext.GetBean<ProcedureChangeScene>().ChangeScene(SceneEnum.GameMain, "GameMain");
                });
        }
    }
}