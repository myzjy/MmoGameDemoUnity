using System;
using System.Collections.Generic;
using GameData.Net;
using Tools.Util;

namespace GameSystem.ServiceDataPro
{
    //最主要的读取存放
    public class GameRootServicePro : BaseServiceData
    {
        private ServiceLoad m_serviceLoad = null;

        public ServiceLoad MServiceLoad
        {
            get { return m_serviceLoad; }
        }

        private static readonly string DefaultApiDomainName = "localhost";

        public string ApiDomainName
        {
            get
            {
#if USE_DEBUG_TOOLS
                return DefaultApiDomainName;
#endif
                return "";
            }
        }

        private static int DefaultApiPort
        {
            get
            {
#if !UNITY_EDITOR
                var port = 443;
                switch ("https")
                {
                    case "https":
                        port = 443;
                        break;
                    case "http":
                        port = 80;
                        break;
                }

                return port;
#else
                //默认443端口
                return 443;

#endif
            }
        }

        private string ApiRootHost
        {
            get
            {
#if USE_DEBUG_TOOLS
                return ApiDomainName;
#endif
                return "";
            }
        }

        public GameRootServicePro()
        {
            //初始化new出来
            SetInfoData();
        }


        //进行设置
        private void SetInfoData()
        {
            SetGameLoad(ApiDomainName, DefaultApiPort);
        }

        private void SetGameLoad(string host, int port)
        {
            if (m_serviceLoad == null)
            {
                m_serviceLoad = new ServiceLoad();
                SetServiceLoad(m_serviceLoad);
            }

            m_serviceLoad.SetURLs(host, port);
        }

        public void SetGameLoad(string url)
        {
            if (m_serviceLoad == null)
            {
                m_serviceLoad = new ServiceLoad();
                SetServiceLoad(m_serviceLoad);
            }

            m_serviceLoad.SetURLs(url);
        }

        public void SetGameLoadPost(string url)
        {
            if (m_serviceLoad == null)
            {
                m_serviceLoad = new ServiceLoad();
                SetServiceLoad(m_serviceLoad);
            }

            m_serviceLoad.SetURLs(url);
        }
    }
}