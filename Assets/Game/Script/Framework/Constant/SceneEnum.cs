﻿using System;
using System.Collections.Generic;
using ZJYFrameWork.Procedure.Scene;

namespace ZJYFrameWork.Constant
{
    public class SceneConstant
    {
        public static readonly string NEXT_SCENE_ENUM = "NextSceneEnum";

        public static readonly Dictionary<SceneEnum, Type> SCENE_MAP = new Dictionary<SceneEnum, Type>()
        {
            { SceneEnum.Login, typeof(ProcedureLogin) },
       
            // { SceneEnum.ActivityScene, typeof(ProcedureActivityScene) }
        };
    }
    public enum SceneEnum
    {
        /// <summary>
        /// 登录
        /// </summary>
        Login = 1,

        /// <summary>
        /// 角色场景
        /// </summary>
        CharacterSelection = 2,

        /// <summary>
        /// 首页
        /// </summary>
        HomeView = 3,

        /// <summary>
        /// 活动场景
        /// </summary>
        ActivityScene = 4,
        /// <summary>
        /// 重复登录
        /// </summary>
        RepeatLogin=5
    }
}