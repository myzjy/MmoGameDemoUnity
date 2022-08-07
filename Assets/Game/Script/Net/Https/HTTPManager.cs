using System;
using BestHTTP;
using BestHTTP.Logger;
using UnityEngine;
using UnityEngine.Networking;

namespace Net.Https
{
    public class HTTPManager
    {
        private static UnityWebRequest _webRequest;
        private static Uri baseUrl;

        /// <summary>
        /// A basic BestHTTP.Logger.ILogger implementation to be able to log intelligently additional informations about the plugin's internal mechanism.
        /// </summary>
        public static BestHTTP.Logger.ILogger Logger
        {
            get
            {
                // Make sure that it has a valid logger instance.
                if (logger == null)
                {
                    logger = new DefaultLogger();
                    logger.Level = Loglevels.None;
                }

                return logger;
            }

            set { logger = value; }
        }

        private static BestHTTP.Logger.ILogger logger;

        public bool UpDate()
        {
            if (_webRequest == null) return false;
            return false;
        }
    }
}