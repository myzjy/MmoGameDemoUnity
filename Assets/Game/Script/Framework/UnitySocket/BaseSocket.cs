using System;
using UnityEngine;

namespace Script.Framework.UnitySocket
{
    [Serializable]
    public abstract class BaseSocket
    {
        public abstract string ToJson();
    }
}