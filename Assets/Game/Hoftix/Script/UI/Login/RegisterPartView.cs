using UnityEngine.UI;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Manager;

namespace ZJYFrameWork.UISerializable
{
    public class RegisterPartView : UISerializableKeyObject
    {
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
            registerAccountInputField = GetObjType<InputField>("registerAccountInputField");
            registerPasswordInputField = GetObjType<InputField>("registerPasswordInputField");
            registerAffirmPasswordInputField = GetObjType<InputField>("registerAffirmPasswordInputField");
            okButton = GetObjType<Button>("okButton");
            cancelButton = GetObjType<Button>("cancelButton");
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(() => { });
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() =>
            {
                //隐藏按钮
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
    }
}