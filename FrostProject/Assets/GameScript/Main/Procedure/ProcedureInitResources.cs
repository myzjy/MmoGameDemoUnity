﻿using System;
using Cysharp.Threading.Tasks;
using FrostEngine;
using UnityEngine;
using ProcedureOwner = FrostEngine.IFsm<FrostEngine.IProcedureManager>;
using Debug = FrostEngine.Debug;

namespace GameMain
{
    public class ProcedureInitResources : ProcedureBase
    {
        private bool m_InitResourcesComplete = false;

        public override bool UseNativeDialog => true;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_InitResourcesComplete = false;
            
            UILoadMgr.Show(UIDefine.UILoadUpdate,"初始化资源中...");
            
            // 注意：使用单机模式并初始化资源前，需要先构建 AssetBundle 并复制到 StreamingAssets 中，否则会产生 HTTP 404 错误
            OnInitResourcesComplete().Forget();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_InitResourcesComplete)
            {
                // 初始化资源未完成则继续等待
                return;
            }

            ChangeState<ProcedurePreload>(procedureOwner);
        }

        private async UniTaskVoid OnInitResourcesComplete()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            Debug.Info("Init resources complete.");
            GameModule.Resource.LoadAsset<ShaderVariantCollection>("MyShaderVariants", res =>
            {
                res.WarmUp();
                m_InitResourcesComplete = true;
            });
        }
    }
}