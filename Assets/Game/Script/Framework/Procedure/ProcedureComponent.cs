using UnityEngine;
using ZJYFrameWork.Base;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.Procedure
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game/FrameWork/Procedure")]
    public sealed class ProcedureComponent : SpringComponent
    {
        [SerializeField] private string[] availableProcedureTypeNames;
        [SerializeField] private string entranceProcedureTypeName;
        [Autowired] private IProcedureFsmManager procedureFsmManager;

        /// <summary>
        /// 启动
        /// </summary>
        public void StartProcedure()
        {
            var entranceProcedureType = AssemblyUtils.GetTypeByName(entranceProcedureTypeName);
            // 初始化流程状态机
            SpringContext.GetBean<IProcedureFsmManager>().ProcedureFsm.Start(entranceProcedureType);
        }
    }
}