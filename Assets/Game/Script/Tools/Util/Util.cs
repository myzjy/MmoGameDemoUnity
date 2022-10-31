using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
// using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Tools.Util
{
    public static class Util
    {
        /// <summary>
        /// button自定义设置
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        public static void SetListener(this Button button,UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }
    }
}