using System;
using System.Collections;
using System.Collections.Generic;
using GameData.Net;
using Script.Config;
using UnityEngine;
using UnityEngine.Networking;

namespace GameSystem.ServiceDataPro
{
    public class GameRootService
    {
        private GameRootServicePro m_gameroot = null;

        public static GameRootService CreateRoot()
        {
            var res = new GameRootService();
            return res;
        }

        private GameRootService()
        {
            //初始化
            m_gameroot = new GameRootServicePro();
        }

        public GameRootServicePro ServicePro => m_gameroot;

        public void HttpsPost(Dictionary<string, string> formData,byte[] byteData,Action<HttpResponse> call,int timeOUt)
        {
            ServicePro.SetGameLoadPost(URLSetting.START_UP_URL);
            ServicePro.RequestSeverPost(URLSetting.START_UP_URL,formData,byteData,call,timeOUt);
        }
      
    }
}