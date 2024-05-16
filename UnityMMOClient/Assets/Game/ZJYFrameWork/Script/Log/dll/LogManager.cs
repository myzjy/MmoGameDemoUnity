using System;
using System.Collections.Generic;
using UnityEngine;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Log
{
    public class LogManager : ILogFactory
    {
        public void Log(Level level, object message)
        {
            switch (level)
            {
                case Level.DEBUG:
                    UnityEngine.Debug.Log(message.ToString());
                    break;

                case Level.INFO:
                    UnityEngine.Debug.Log(message.ToString());
                    break;

                case Level.WARN:
                    UnityEngine.Debug.LogWarning(message.ToString());
                    break;

                case Level.ERROR:
                    UnityEngine.Debug.LogError(message.ToString());
                    break;

                default:
                    throw new Exception(message.ToString());
            }
        }
    }
}