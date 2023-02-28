using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Debugger.Widows.Model;
using ZJYFrameWork.Prefs;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Setting
{
    [Bean]
    public class PlayerPrefsSettingHelper : ISettingHelper
    {
        private Preferences _preferences;
        private string name;
        protected static readonly string GLOBAL_NAME = "_GLOBAL_";

        [BeforePostConstruct]
        private void Init()
        {
            _preferences = Preferences.GetGlobalPreferences();
            Debug.Log(_preferences);
            if (string.IsNullOrEmpty(this.name))
                this.name = GLOBAL_NAME;
        }

        public bool Load()
        {
            return true;
        }

        public bool Save()
        {
            PlayerPrefs.Save();

            return true;
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <returns>所有游戏配置项的名称。</returns>
        public string[] GetAllSettingNames()
        {
            Debug.LogError("GetAllSettingNames is not supported.");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string Key(string key)
        {
            StringBuilder buf = new StringBuilder(this.Name);
            buf.Append(".").Append(key);
            return buf.ToString();
        }

        /// <summary>
        /// The name of the preferences
        /// </summary>
        public string Name
        {
            get { return this.name; }
            protected set { this.name = value; }
        }

        /// <summary>
        /// 检查是否存在指定游戏配置项。
        /// </summary>
        /// <param name="settingName">要检查游戏配置项的名称。</param>
        /// <returns>指定的游戏配置项是否存在。</returns>
        public bool HasSetting(string settingName)
        {
            Debug.Log(Key(settingName));
            var keys = Key(settingName);
            return !string.IsNullOrEmpty(PlayerPrefs.GetString(keys));
        }

        /// <summary>
        /// 移除指定游戏配置项。
        /// </summary>
        /// <param name="settingName">要移除游戏配置项的名称。</param>
        /// <returns>是否移除指定游戏配置项成功。</returns>
        public bool RemoveSetting(string settingName)
        {
            if (!HasSetting(settingName))
            {
                return false;
            }

            var keys = Key(settingName);
            PlayerPrefs.DeleteKey(keys);
            return true;
        }

        /// <summary>
        /// 清空所有游戏配置项。
        /// </summary>
        public void RemoveAllSettings()
        {
            //移除
            PlayerPrefs.DeleteAll();
            // _preferences.RemoveAll();
        }

        /// <summary>
        /// 从指定游戏配置项中读取布尔值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string settingName)
        {
            return GetObject<bool>(settingName);
        }

        /// <summary>
        /// 从指定游戏配置项中读取布尔值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string settingName, bool defaultValue)
        {
            return GetObject(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入布尔值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的布尔值。</param>
        public void SetBool(string settingName, bool value)
        {
            var key = Key(settingName);

            SetObject(key, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取整数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string settingName)
        {
            return GetObject<int>(settingName);
        }

        /// <summary>
        /// 从指定游戏配置项中读取整数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string settingName, int defaultValue)
        {
            return GetObject(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入整数值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的整数值。</param>
        public void SetInt(string settingName, int value)
        {
            var key = Key(settingName);

            SetObject(key, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取浮点数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string settingName)
        {
            return GetObject<float>(settingName);
        }

        /// <summary>
        /// 从指定游戏配置项中读取浮点数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string settingName, float defaultValue)
        {
            return GetObject(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入浮点数值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的浮点数值。</param>
        public void SetFloat(string settingName, float value)
        {
            var key = Key(settingName);

            SetObject(key, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取字符串值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string settingName)
        {
            var key = Key(settingName);
            if (!HasSetting(settingName))
                return "";

            string str = PlayerPrefs.GetString(Key(key));
            return str;
        }

        /// <summary>
        /// 从指定游戏配置项中读取字符串值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string settingName, string defaultValue)
        {
            var key = Key(settingName);
            if (!HasSetting(settingName))
                return defaultValue;

            string str = PlayerPrefs.GetString(Key(key));
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            return str;
        }

        /// <summary>
        /// 向指定游戏配置项写入字符串值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的字符串值。</param>
        public void SetString(string settingName, string value)
        {
            var key = Key(settingName);
            string str = value == null ? "" : JsonConvert.SerializeObject(value);


            PlayerPrefs.SetString(key, str);
        }

        /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <typeparam name="T">要读取对象的类型。</typeparam>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <returns>读取的对象。</returns>
        public T GetObject<T>(string settingName)
        {
            return JsonConvert.DeserializeObject<T>(GetString(settingName));
        }

        /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <typeparam name="T">要读取对象的类型。</typeparam>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultObj">当指定的游戏配置项不存在时，返回此默认对象。</param>
        /// <returns>读取的对象。</returns>
        public T GetObject<T>(string settingName, T defaultObj)
        {
            var json = GetString(settingName, null);
            return json == null ? defaultObj : JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <typeparam name="T">要写入对象的类型。</typeparam>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        public void SetObject<T>(string settingName, T obj)
        {
            SetString(settingName, JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        public void SetObject(string settingName, object obj)
        {
            SetString(settingName, JsonConvert.SerializeObject(obj));
        }
    }
}