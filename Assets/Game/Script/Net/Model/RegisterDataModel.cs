namespace ZJYFrameWork.Net
{
    public partial class RegisterDataModel : Model
    {
        public string platformId { get; set; }
        public string platfromToken { get; set; }
        public string channelCode { get; set; }
        public string version { get; set; }


        public override string ToJson(bool isPretty = false)
        {
            return "";
        }
    }
}