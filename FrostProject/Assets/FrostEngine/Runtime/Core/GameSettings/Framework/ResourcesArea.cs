using System;
using UnityEngine;

namespace FrostEngine
{
    /// <summary>
    /// 资源存放地址。
    /// </summary>
    [Serializable]
    public class ResourcesArea
    {
        [Tooltip("资源管理类型")] [SerializeField] private string m_ResAdminType = "Default";

        /// <summary>
        /// 资源管理类型。
        /// </summary>
        public string ResAdminType => m_ResAdminType;

        [Tooltip("资源管理编号")] [SerializeField] private string m_ResAdminCode = "0";

        /// <summary>
        /// 资源管理编号。
        /// </summary>
        public string ResAdminCode => m_ResAdminCode;

        [Tooltip("服务器类型")] [SerializeField] private HostType m_ServerType = HostType.Develop;

        /// <summary>
        /// 服务器类型。
        /// </summary>
        public HostType ServerType => m_ServerType;

        [Tooltip("是否在构建资源的时候清理上传到服务端目录的老资源")] [SerializeField]
        private bool m_CleanCommitPathRes = true;

        public bool CleanCommitPathRes => m_CleanCommitPathRes;

        [Tooltip("内网地址")] [SerializeField] private string m_InnerResourceSourceUrl = "http://127.0.0.1:8088";

        public string InnerResourceSourceUrl => m_InnerResourceSourceUrl;

        [Tooltip("外网地址")] [SerializeField] private string m_ExtraResourceSourceUrl = "http://127.0.0.1:8088";

        public string ExtraResourceSourceUrl => m_ExtraResourceSourceUrl;

        [Tooltip("正式地址")] [SerializeField] private string m_FormalResourceSourceUrl = "http://127.0.0.1:8088";

        public string FormalResourceSourceUrl => m_FormalResourceSourceUrl;
    }
}