using System;
using System.Collections.Generic;
using UnityEngine;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.I18n
{
    [Bean]
    public class I18nManager : II18nManager
    {
        private static readonly string LANGUAGES_TAG = "languages";
        private static readonly string LANGUAGE_TAG = "language";
        private static readonly string LANGUAGE_ATTRIBUTE = "name";
        private static readonly string PAIR_TAG = "pair";
        private static readonly string PAIR_KEY = "key";
        private static readonly string PAIR_VALUE = "value";
        private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);

        private readonly List<SystemLanguage> supportedLanguages = new List<SystemLanguage>();
        public SystemLanguage language { get; set; }
        public object mainFontAsset { get; set; }

        public List<SystemLanguage> GetSupportedLanguages()
        {
            return supportedLanguages;
        }

        public string GetString(string key)
        {
            throw new System.NotImplementedException();
        }

        public string GetString(string key, object arg0)
        {
            throw new System.NotImplementedException();
        }

        public string GetString(string key, object arg0, object arg1)
        {
            throw new System.NotImplementedException();
        }

        public string GetString(string key, object arg0, object arg1, object arg2)
        {
            throw new System.NotImplementedException();
        }

        public string GetString(string key, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public bool AddString(string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public bool ParseSupportedLanguages(string dictionaryString)
        {
            throw new System.NotImplementedException();
        }

        public bool ParseData(string dictionaryString)
        {
            throw new System.NotImplementedException();
        }
    }
}