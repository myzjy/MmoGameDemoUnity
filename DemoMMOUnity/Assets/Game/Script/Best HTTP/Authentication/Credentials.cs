namespace BestHTTP.Authentication
{
    /// <summary>
    /// bestHttp支持的身份验证类型。
    /// 身份验证是由服务器定义的，因此基本和摘要是不可互换的
    /// 如果您不知道使用什么，首选的方法是选择Unknown。
    /// </summary>
    public enum AuthenticationTypes
    {
        /// <summary>
        /// 如果身份验证类型未知，这将做一个挑战，以接收应该选择的方法.
        /// </summary>
        Unknown,

        /// <summary>
        /// 最基本的身份验证类型。这很容易做到，也很容易破解. ;)
        /// </summary>
        Basic,

        /// <summary>
        /// 
        /// </summary>
        Digest
    }

    /// <summary>
    /// 保存向远程服务器进行身份验证所需的所有信息.
    /// </summary>
    public sealed class Credentials
    {
        /// <summary>
        /// 使用用户名和密码设置身份验证凭据。类型将被设置为未知。
        /// </summary>
        public Credentials(string userName, string password)
            : this(
                type: AuthenticationTypes.Unknown,
                userName: userName,
                password: password)
        {
        }

        /// <summary>
        /// 使用给定的身份验证类型、用户名和密码设置身份验证凭据。
        /// </summary>
        private Credentials(AuthenticationTypes type, string userName, string password)
        {
            this.Type = type;
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>
        /// 认证的类型。如果你不知道该用什么，首选的方法是选择 Unknown.
        /// </summary>
        public AuthenticationTypes Type { get; private set; }

        /// <summary>
        /// 在远程服务器上进行身份验证的用户名。
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 身份验证过程中使用的密码。密码将只存储在这个类中。
        /// </summary>
        public string Password { get; private set; }
    }
}