using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameTools.Singletons;
using ZJYFrameWork;
using UnityEngine;

namespace ZJYFrameWork.Framwork.Times
{
    public class CoroutineManager : MMOSingletonDontDestroy<CoroutineManager>
    {
        private LinkedList<IEnumerator> coroutineList = new LinkedList<IEnumerator>();

        public void StartCoroutine(IEnumerator enumerator)
        {
            coroutineList.AddLast(enumerator);
        }

        public void StopCoroutine(IEnumerator enumerator)
        {
            try
            {
                coroutineList.Remove(enumerator);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError($"{enumerator.ToString()}  删除错误 没有添加");
#endif
            }
        }

        public void UpDataCoroutoine()
        {
            var node = coroutineList.First;
            if (node != null)
            {
                IEnumerator ie = node.Value;
                bool ret = true;
            }
        }

        
    }
}