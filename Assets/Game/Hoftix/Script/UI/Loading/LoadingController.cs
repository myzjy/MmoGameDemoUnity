using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.UISerializable
{
    public class LoadingController : MonoBehaviour
    {
        
        public void Build()
        {
            SpringContext.RegisterBean(this);
        }
    }
}