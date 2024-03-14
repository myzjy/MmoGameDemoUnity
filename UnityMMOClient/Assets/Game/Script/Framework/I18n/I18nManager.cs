using System;
using System.Collections.Generic;
using UnityEngine;
using ZJYFrameWork.AssetBundles;
using ZJYFrameWork.AssetBundles.IAssetBundlesManagerInterface;
using ZJYFrameWork.AssetBundles.Model;
using ZJYFrameWork.AssetBundles.Model.Callback;
using ZJYFrameWork.Common;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.I18n
{
    [Bean]
    public class I18nManager : II18nManager
    {
#pragma warning disable CS0414
        private static readonly string LANGUAGES_TAG = "languages";
#pragma warning restore CS0414
#pragma warning disable CS0414
        private static readonly string LANGUAGE_TAG = "language";
#pragma warning restore CS0414
#pragma warning disable CS0414
        private static readonly string LANGUAGE_ATTRIBUTE = "name";
#pragma warning restore CS0414
#pragma warning disable CS0414
        private static readonly string PAIR_TAG = "pair";
#pragma warning restore CS0414
#pragma warning disable CS0414
        private static readonly string PAIR_KEY = "key";
#pragma warning restore CS0414
#pragma warning disable CS0414
        private static readonly string PAIR_VALUE = "value";
#pragma warning restore CS0414
        private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);

        private readonly List<SystemLanguage> supportedLanguages = new List<SystemLanguage>();

        private LoadAssetCallbacks _loadAssetCallbacks;

        private bool isLoad = false;
        private bool isLoadAsset = false;
        public SystemLanguage language { get; set; }
        public object mainFontAsset { get; set; }

        public List<SystemLanguage> GetSupportedLanguages()
        {
            return supportedLanguages;
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <param name="key">字典主键。</param>
        /// <returns>要获取的字典内容字符串。</returns>
        public string GetString(string key)
        {
            string value = GetValue(key);
            if (value == null)
            {
                return StringUtils.Empty;
            }

            return value;
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <param name="key">字典主键。</param>
        /// <param name="arg0">字典参数 0。</param>
        /// <returns>要获取的字典内容字符串。</returns>
        public string GetString(string key, object arg0)
        {
            string value = GetValue(key);
            if (value == null)
            {
                return StringUtils.Format("<NoKey>{}", key);
            }

            try
            {
                return StringUtils.Format(value, arg0);
            }
            catch (Exception exception)
            {
                return StringUtils.Format("<Error>{},{},{},{}", key, value, arg0, exception.ToString());
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <param name="key">字典主键。</param>
        /// <param name="arg0">字典参数 0。</param>
        /// <param name="arg1">字典参数 1。</param>
        /// <returns>要获取的字典内容字符串。</returns>
        public string GetString(string key, object arg0, object arg1)
        {
            string value = GetValue(key);
            if (value == null)
            {
                return StringUtils.Format("<NoKey>{}", key);
            }

            try
            {
                return StringUtils.Format(value, arg0, arg1);
            }
            catch (Exception exception)
            {
                return StringUtils.Format("<Error>{},{},{},{},{}", key, value, arg0, arg1, exception.ToString());
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <param name="key">字典主键。</param>
        /// <param name="arg0">字典参数 0。</param>
        /// <param name="arg1">字典参数 1。</param>
        /// <param name="arg2">字典参数 2。</param>
        /// <returns>要获取的字典内容字符串。</returns>
        public string GetString(string key, object arg0, object arg1, object arg2)
        {
            string value = GetValue(key);
            if (value == null)
            {
                return StringUtils.Format("<NoKey>{}", key);
            }

            try
            {
                return StringUtils.Format(value, arg0, arg1, arg2);
            }
            catch (Exception exception)
            {
                return StringUtils.Format("<Error>{},{},{},{},{},{}", key, value, arg0, arg1, arg2,
                    exception.ToString());
            }
        }

        public string GetString(string key, params object[] args)
        {
            string value = GetValue(key);
            if (value == null)
            {
                return StringUtils.Format("<NoKey>{}", key);
            }

            try
            {
                return StringUtils.Format(value, args);
            }
            catch (Exception exception)
            {
                string errorString = StringUtils.Format("<Error>{},{}", key, value);
                if (args != null)
                {
                    foreach (object arg in args)
                    {
                        errorString += "," + arg.ToString();
                    }
                }

                errorString += "," + exception.ToString();
                return errorString;
            }
        }

        public bool AddString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key is invalid.");
            }

            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            dictionary.Add(key, value ?? string.Empty);
            return true;
        }

        public bool ParseSupportedLanguages(string dictionaryString)
        {
            throw new System.NotImplementedException();
        }

        public bool ParseData(string dictionaryString)
        {
            if (isLoadAsset)
            {
                return true;
            }

            if (isLoad)
            {
                return true;
            }

            isLoad = true;
            SpringContext.GetBean<IAssetBundleManager>().LoadAsset(dictionaryString, _loadAssetCallbacks);
            return true;
        }

        [PostConstruct]
        public void Init()
        {
            _loadAssetCallbacks =
                new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback, null, null);
        }

        /// <summary>
        /// 根据字典主键获取字典值。
        /// </summary>
        /// <param name="key">字典主键。</param>
        /// <returns>字典值。</returns>
        public string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key is invalid.");
            }

            return dictionary.GetValueOrDefault(key);
        }

        private void LoadAssetSuccessCallback(string assetName, UnityEngine.Object asset, float duration,
            object userData)
        {
            isLoadAsset = true;
            MessageData messageData = asset as MessageData;
            if (messageData != null)
            {
                var listString = messageData.messages;
                string[] arrNames = Enum.GetNames(typeof(Message));
                for (int i = 0; i < listString.Length; i++)
                {
                    var key = arrNames[i];
                    var value = listString[i];
                    AddString(key, value);
                }
            }
        }

        private void LoadAssetFailureCallback(string soundAssetName, LoadResourceStatus status, string errorMessage,
            object userData)
        {
        }
    }
}