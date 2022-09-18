using System.Text;
using Newtonsoft.Json;

namespace ZJYFrameWork.Net
{
	public  class ApiLoginServerData:ApiHttp<ApiLoginServerDataRequest,ApiLoginServerDataResponse,Error>
	{
		public ApiLoginServerData()
		{
		}
	}
}