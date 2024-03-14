using System;
using System.Linq;
using UnityEngine;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.Base.Component
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
            var subTypeList = AssemblyUtils.GetAllSubClassType(typeof(SpringComponent));
            var list = SpringContext.GetAllBeans();
            var startSpringFlag = subTypeList.All(subType => list.Any(it => it.GetType() == subType));

            if (!startSpringFlag) return;
            SpringContext.GetBean<BaseComponent>().StartSpring();
        }
    }
}