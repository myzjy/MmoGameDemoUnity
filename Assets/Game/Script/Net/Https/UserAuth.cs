using System;
using UnityEngine;

namespace Net.Https
{
    [Serializable]
    public class UserAuth
    {
        [SerializeField] private string authToken = "";


        public string AuthToken
        {
            get { return authToken; }
            set { authToken = value; }
        }

        public UserAuth()
        {
        }

        public override string ToString()
        {
            return AuthToken;
        }
        
    }
}