using System;
using System.Linq;
using UnityEngine;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.Base
{
    /// <summary>
    /// 游戏基础组件 抽象类
    /// </summary>
    public abstract class SpringComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            //注册
            SpringContext.RegisterBean(this);
            var startSpringFlag = true;
            var subTypeList = AssemblyUtils.GetAllSubClassType(typeof(SpringComponent));
            var list = SpringContext.GetAllBeans();
            foreach (var subType in subTypeList)
            {
                if (list.Any(it =>
                    {
                        Debug.Log($"it.GetType():{it.GetType()}");
                        Debug.Log($"subType:{subType}");
                        return it.GetType() == subType;
                    }))
                {
                    continue;
                }

                startSpringFlag = false;
                break;
            }

            if (!startSpringFlag) return;
            Debug.Log("开始扫描");
            SpringContext.GetBean<BaseComponent>().StartSpring();
        }
    }
}