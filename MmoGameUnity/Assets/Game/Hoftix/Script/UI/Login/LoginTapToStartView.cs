using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Procedure.Scene;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Common;

namespace ZJYFrameWork.UISerializable
{
    public class LoginTapToStartView : UISerializableKeyObject
    {
        public Button LoginStartMaxButton = null;
        public Button LoginStartButton = null;
        public LoginView LoginView = null;

        /// <summary>
        /// 赋值 构建 传递过来LoginView，便于控制
        /// </summary>
        /// <param name="loginView"></param>
        public void Build(LoginView loginView)
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
                LoginStartMaxButton.SetListener(() => { SpringContext.GetBean<LoginController>().OnHide(); });
            }
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            gameObject.SetActive(false);
        }

        public void Show()
        {
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() => {  CommonController.Instance.loadingRotate.OnShow(); });
            sequence.AppendInterval(1.5f);
            sequence.AppendCallback(() => {  CommonController.Instance.loadingRotate.OnClose(); });
            sequence.AppendInterval(0.5f);

            sequence.AppendCallback(() =>
            {
                gameObject.SetActive(true);

                CommonController.Instance.loadingRotate.OnClose();
                var buttonTapToStartCanvasGroup = GetComponentInChildren<CanvasGroup>();
                buttonTapToStartCanvasGroup.DOFade(1f, 1.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            });
        }

        public void StartGame()
        {
            var buttonTapToStartCanvasGroup = GetComponentInChildren<CanvasGroup>();
            buttonTapToStartCanvasGroup.DOKill();
            buttonTapToStartCanvasGroup.DOFade(1f, 0.2f).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    if (LoginView == null)
                    {
#if DEVELOP_BUILD
                        Debug.LogError(
                            $"[LoginView] 组件为空,请检查Build方法,检查[LoginView]中方法[OnInit]有没有进行build方法调度,有没有进行赋值,流程无法继续");
#endif
                        //流程卡死了，不能隐藏
                        Show();
                        return;
                    }

                    SpringContext.GetBean<LoginController>().OnHide();
                    //跳转场景 
                    SpringContext.GetBean<ProcedureChangeScene>().ChangeScene(SceneEnum.GameMain, "GameMain");

                    // PlayManager.Instance.LoadScene(Data.scene_home);
                    // SpringContext.GetBean<ProcedureChangeScene>().ChangeScene(SceneEnum.Menu);
                    // LoginController.GetInstance().DestroyThis();
                });
        }
    }
}