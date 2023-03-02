using System;
using ZJYFrameWork.Common;
using ZJYFrameWork.Constant;
using ZJYFrameWork.I18n;
using ZJYFrameWork.Net.CsProtocol;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Common;

namespace ZJYFrameWork.Module.System.Controller
{
    [Bean]
    public class SystemController
    {
        [Autowired] private II18nManager i18nManager;

        [Autowired] private ISettingManager settingManager;

        [Autowired] private LoginClientCacheData _clientCacheData;

        [PacketReceiver]
        public void AtError(Error error)
        {
            var errorMessage = error.errorMessage;

            if (StringUtils.IsBlank(errorMessage))
            {
                return;
            }

            var errorString = i18nManager.GetString(errorMessage);
            CommonController.Instance.snackbar.SeverError(errorString);
            var i18nEnum = Message.LANGUAGE_CODE;
            Enum.TryParse<Message>(errorMessage, out i18nEnum);
            switch (i18nEnum)
            {
                case Message.LANGUAGE_CODE:
                    return;
                case Message.error_account_not_blank: //输入账号空的
                case Message.error_account_not_exit: //账号出错 输入账号 不存在，或者没有注册

                    _clientCacheData.loginError = true;
                    return;
                case Message.error_account_password:
                    break;
                case Message.error_protocol_param:
                    break;
                case Message.error_user_orm_no_uid:
                    break;
                case Message.error_account_password_not_affirm:
                    break;
                case Message.error_account_already_exists:
                    break;
                case Message.error_password_length:
                    break;
                case Message.error_password_not_have_null:
                    break;
                case Message.MAX_NUM:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}