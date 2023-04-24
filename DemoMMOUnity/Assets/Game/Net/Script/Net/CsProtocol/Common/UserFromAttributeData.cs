namespace ZJYFrameWork.Net.CsProtocol
{
    public class UserFromAttributeData
    {
        public string fromCode;
        public string fromParam;
        public string fromAffiliate;

        public int channelCode;
        public string platformId;
        public string sdkToken;

        public void printData()
        {
            Debug.Log("fromCode: " + fromCode + ", fromParam: " + fromParam + ", fromAffiliate: " + fromAffiliate);
        }
    }
}