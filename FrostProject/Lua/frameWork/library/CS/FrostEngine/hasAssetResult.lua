---@class CS.FrostEngine.HasAssetResult
CS.FrostEngine.HasAssetResult = {
    ---资源不存在。
    NotExist = 0,
   
    ---资源需要从远端更新下载。
    AssetOnline = 1,

    ---存在资源且存储在磁盘上。
    AssetOnDisk = 2,
   
    ---存在资源且存储在文件系统里。
    AssetOnFileSystem = 3,

    ---存在二进制资源且存储在磁盘上。
    BinaryOnDisk = 4,
   
    ---存在二进制资源且存储在文件系统里。
    BinaryOnFileSystem = 5,
   
    ---资源定位地址无效。
    Valid = 6,
}