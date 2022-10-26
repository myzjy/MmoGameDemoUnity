namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class RegisterRequest : IPacket
    {
        /// <summary>
        /// 输入账号名
        /// </summary>
        public string Account;

        /// <summary>
        /// 输入密码
        /// </summary>
        public string Password;

        /// <summary>
        /// 确认密码
        /// </summary>
        public string AffirmPassword;

        public RegisterRequest ValueOf(string account, string password, string affirmPassword)
        {
            var packet = new RegisterRequest()
            {
                Account = account,
                Password = password,
                AffirmPassword = affirmPassword
            };
            return packet;
        }


        public short ProtocolId()
        {
            return 1006;
        }
    }
}