﻿namespace ZJYFrameWork.Debugger.Widows.Model
{
    /// <summary>
    /// 调试器窗口组接口
    /// </summary>
    public interface IDebuggerWindowGroup
    {
        /// <summary>
        /// 获取调试器窗口数量
        /// </summary>
        int DebuggerWindowsCount { get; }

        /// <summary>
        /// 获取或设置当前选中的调试器窗口索引
        /// </summary>
        int SelectedIndex { get; set; }

        /// <summary>
        /// 获取当前选中的调试器窗口。
        /// </summary>
        IDebuggerWindow SelectedWindow { get; }

        /// <summary>
        /// 获取调试组的调试器窗口名称集合。
        /// </summary>
        string[] GetDebuggerWindowNames();

        /// <summary>
        /// 获取调试器窗口。
        /// </summary>
        /// <param name="path">调试器窗口路径。</param>
        /// <returns>要获取的调试器窗口。</returns>
        IDebuggerWindow GetDebuggerWindow(string path);

        /// <summary>
        /// 注册调试器窗口。
        /// </summary>
        /// <param name="path">调试器窗口路径。</param>
        /// <param name="debuggerWindow">要注册的调试器窗口。</param>
        void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow);
    }
}