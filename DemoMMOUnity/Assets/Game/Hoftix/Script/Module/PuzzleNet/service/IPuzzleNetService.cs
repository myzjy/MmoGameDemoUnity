namespace ZJYFrameWork.Module.PuzzleNet.service
{
    /// <summary>
    /// 地图 map 网络请求 service 接口
    /// </summary>
    public interface IPuzzleNetService
    {
        /// <summary>
        /// 获取地图总数据表 服务
        /// </summary>
        /// <param name="eventID">事件id或者活动id，大地图id</param>
        void GetTheMapTotalDataTableService(int eventID);
    }
}