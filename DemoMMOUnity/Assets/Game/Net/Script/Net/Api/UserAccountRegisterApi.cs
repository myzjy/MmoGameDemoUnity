/*
 * 【注意事项】本文档，是由服务器的http:generate自动生成的代码，因此请勿在此追加hash任何逻辑。
 */
using ZJYFrameWork.Net;
using BestHTTP;

namespace ZJYFrameWork.Net
{
	/// <summary>
	/// 账号注册
	/// </summary>
	public class UserAccountRegisterApi : ApiHttp<UserAccountRegisterRequest, UserAccountRegisterResponse, Error>
	{
		public UserAccountRegisterApi()
		{
			this.Method = BestHTTP.HttpMethods.Get;
			this.Path = "/api/user-account/register";
			this.Param = new UserAccountRegisterRequest();

			this.authorize = false;
			this.ignoreVerify = false;
			this.ignoreError = false;
		}
	}
}
