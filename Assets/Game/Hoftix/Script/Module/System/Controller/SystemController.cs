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

namespace ZJYFrameWork.Module.System.Controller
{
    [Bean]
    public class SystemController
    {
        [Autowired]
        private II18nManager i18nManager;

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

            CommonController.Instance.snackbar.SeverError(errorMessage);
            var i18nEnum = I18nEnum.default_enum;
            Enum.TryParse<I18nEnum>(errorMessage, out i18nEnum);
            if (i18nEnum == I18nEnum.default_enum)
            {
                return;
            }

            if (i18nEnum == I18nEnum.error_account_not_exit)
            {
                _clientCacheData.loginError = true;
                return ;
            }
            
            
       
        }
    }
}