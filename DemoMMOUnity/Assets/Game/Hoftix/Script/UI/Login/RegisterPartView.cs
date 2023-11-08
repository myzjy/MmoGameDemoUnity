﻿using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Event;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Module.Register.Service;
using ZJYFrameWork.Net;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Common;
using ZJYFrameWork.UISerializable.Manager;
using ZJYFrameWork.WebRequest;

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
            rootCanvasGroup = GetObjType<CanvasGroup>("root_CanvasGroup");
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
#if HTTP_SEND_OPEN
            UserAccountRegisterApi registerApi = new UserAccountRegisterApi()
            {
                onBeforeSend = () => { CommonController.Instance.loadingRotate.OnShow(); },
                onComplete = () => { CommonController.Instance.loadingRotate.OnClose(); },
                onSuccess = res =>
                {
                    //重新打开登录面板
                    SpringContext.GetBean<LoginUIController>().OnInit();
                },
                Param =
                {
                    Account = accountString,
                    Password = passwordString
                }
            };
            registerApi.Param.Account = accountString;
            registerApi.Param.Password = passwordString;
            registerApi.Param.AffirmPassword = affirmPasswordString;
            SpringContext.GetBean<NetworkManager>().Request(registerApi);
#else
            SpringContext.GetBean<IRegisterService>().RegisterAccount();
#endif
        }

        public void OnShow()
        {
            rootCanvasGroup.DOKill();
            rootCanvasGroup.DOFade(0f, 0f);
            rootCanvasGroup.DOFade(1f, 0.2f).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    rootCanvasGroup.interactable = true;
                    rootCanvasGroup.blocksRaycasts = true;
                    // PlayManager.Instance.LoadScene(Data.scene_home);
                    rootObj.SetActive(true);
                });
        }

        public void OnClose()
        {
            if (!rootObj.activeSelf)
            {
                return;
            }

            rootCanvasGroup.DOKill();
            rootCanvasGroup.DOFade(0f, 0f);
            rootCanvasGroup.DOFade(0f, 0.2f).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    rootCanvasGroup.interactable = false;
                    rootCanvasGroup.blocksRaycasts = false;
                    // PlayManager.Instance.LoadScene(Data.scene_home);
                    rootObj.SetActive(false);
                    //打开登录界面
                    SpringContext.GetBean<LoginUIController>().loginPartView.Show();
                });
        }
    }
}