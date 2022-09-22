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
            //注册
            SpringContext.RegisterBean(this);
            var startSpringFlag = true;
            var subTypeList = AssemblyUtils.GetAllSubClassType(typeof(SpringComponent));
            foreach (var subType in subTypeList)
            {
                if (SpringContext.GetAllBeans().Any(it => it.GetType() == subType))
                {
                    continue;
                }

                startSpringFlag = false;
                break;
            }
            if (startSpringFlag)
            {
                SpringContext.GetBean<BaseComponent>().StartSpring();
            }
        }
    }
}