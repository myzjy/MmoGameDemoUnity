using System;
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
            { SceneEnum.GameMain, typeof(ProcedureGameMain) }
        };
    }

    public enum SceneEnum
    {
        /// <summary>
        /// 登录
        /// </summary>
        Login = 1,

        /// <summary>
        /// 主界面
        /// </summary>
        GameMain = 2,

        /// <summary>
        /// 重复登录
        /// </summary>
        RepeatLogin = 3
    }
}