/*
 * 【注意事项】本文档，是由服务器的http:generate自动生成的代码，因此请勿在此追加hash任何逻辑。
 */

using ZJYFrameWork.Net;
using BestHTTP;

namespace ZJYFrameWork.Net
{
    /// <summary>
    /// 使用体力
    /// </summary>
    public class PhysicalPowerUsePropsPhysicalPowerUserPropsApi : ApiHttp<
        PhysicalPowerUsePropsPhysicalPowerUserPropsRequest, PhysicalPowerUsePropsPhysicalPowerUserPropsResponse, Error>
    {
        public PhysicalPowerUsePropsPhysicalPowerUserPropsApi()
        {
            this.Method = BestHTTP.HttpMethods.Get;
            this.Path = "/api/physical-power-use-props/physical-power-user-props";
            this.Param = new PhysicalPowerUsePropsPhysicalPowerUserPropsRequest();

            this.authorize = false;
            this.ignoreVerify = false;
            this.ignoreError = false;
        }
    }
}