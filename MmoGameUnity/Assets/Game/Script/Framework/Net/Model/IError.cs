namespace ZJYFrameWork.Net
{
    public interface IError
    {
        /// <summary>
        /// 出现错误的CommandID
        /// </summary>
        int commandId { get; set; }

        /// <summary>
        /// 具体错误信息
        /// </summary>
        string message { get; set; }
    }
}