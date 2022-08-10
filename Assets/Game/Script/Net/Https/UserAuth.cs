namespace Net.Https
{
    public class UserAuth
    {
        private string authToken = "";

        public string AuthToken
        {
            get { return authToken; }
            set
            {
                authToken = value;
                
            }
        }

        public UserAuth()
        {
            
        }
    }
}