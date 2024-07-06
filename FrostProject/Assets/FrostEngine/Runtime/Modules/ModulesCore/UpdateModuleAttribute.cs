using System;

namespace FrostEngine
{
    /// <summary>
    /// 模块需要框架轮询属性。
    /// </summary>
    /// <remarks> 注入此属性标识模块需要轮询。</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    internal class UpdateModuleAttribute : Attribute
    {
        
    }
}