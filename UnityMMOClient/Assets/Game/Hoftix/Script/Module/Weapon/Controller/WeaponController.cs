using FrostEngine;
using ZJYFrameWork.Net.CsProtocol.Buffer.Protocol.Weapon;
using ZJYFrameWork.Net.Dispatcher;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Module.System.Controller
{
    /// <summary>
    /// 武器的控制
    /// </summary>
    [Bean]
    public class WeaponController
    {
        [PacketReceiver]
        public void AtWeaponUsePlayerResponse(WeaponUsePlayerResponse response)
        {
#if UNITY_EDITOR||(DEVELOP_BUILD && ENABLE_LOG)
            Debug.Log("调用到 at WeaponUsePlayerResponse");
#endif
        }
    }
}