using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Event;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Manager;

namespace ZJYFrameWork.UISerializable
{
    public class RegisterPartView : UISerializableKeyObject
    {
        /// <summary>
        /// root节点CanvasGroup
        /// </summary>
        public CanvasGroup rootCanvasGroup;

        /// <summary>
        /// rootTransform
        /// </summary>
        public Transform root;

        /// <summary>
        /// root节点
        /// </summary>
        public GameObject rootObj;

        /// <summary>
        /// 输入账号
        /// </summary>
        public InputField registerAccountInputField;

        /// <summary>
        /// 输入密码
        /// </summary>
        public InputField registerPasswordInputField;

        /// <summary>
        /// 确认密码
        /// </summary>
        public InputField registerAffirmPasswordInputField;

        /// <summary>
        /// 确认账号注册
        /// </summary>
        public Button okButton;


        public Button cancelButton;
        private long clickLoginTime;

        public void Build()
        {
            rootCanvasGroup = GetObjType<CanvasGroup>("rootCanvasGroup");
            rootObj = GetObjType<GameObject>("root");
            root = rootObj.transform;
            registerAccountInputField = GetObjType<InputField>("registerAccountInputField");
            registerPasswordInputField = GetObjType<InputField>("registerPasswordInputField");
            registerAffirmPasswordInputField = GetObjType<InputField>("registerAffirmPasswordInputField");
            okButton = GetObjType<Button>("okButton");
            cancelButton = GetObjType<Button>("cancelButton");
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(() => { OnClickRegister(); });
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() =>
            {
                //隐藏按钮
                OnClose();
            });
        }

        private void OnClickRegister()
        {
            if (DateTimeUtil.CurrentTimeMillis() - clickLoginTime < DateTimeUtil.CLICK_INTERVAL)
            {
                return;
            }

            Debug.Log("账号密码注册[account:{}][password:{}][affirmPassword:{}]", registerAccountInputField.text,
                registerPasswordInputField.text, registerAffirmPasswordInputField.text);
            var accountString = registerAccountInputField.text;
            var passwordString = registerPasswordInputField.text;
            var affirmPasswordString = registerPasswordInputField.text;
            SpringContext.GetBean<ServerDataManager>()
                .SetCacheRegisterAccountAndPassword(accountString, passwordString, affirmPasswordString);
          
        }

        public void OnShow()
        {
            rootObj.SetActive(true);
            rootCanvasGroup.DOKill();
            rootCanvasGroup.DOFade(0f, 0f);
            rootCanvasGroup.DOFade(1f, 0.2f).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    rootCanvasGroup.interactable = true;
                    rootCanvasGroup.blocksRaycasts = true;
                    // PlayManager.Instance.LoadScene(Data.scene_home);
                    // rootObj.SetActive(fal);
                });
        }

        public void OnClose()
        {
            if (!rootObj.activeSelf)
            {
                return;
            }
            rootCanvasGroup.DOKill();
            rootCanvasGroup.DOFade(1f, 0f);
            rootCanvasGroup.DOFade(0f, 0.2f).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    rootCanvasGroup.interactable = false;
                    rootCanvasGroup.blocksRaycasts = false;
                    // PlayManager.Instance.LoadScene(Data.scene_home);
                    rootObj.SetActive(false);
                });
        }
    }
}