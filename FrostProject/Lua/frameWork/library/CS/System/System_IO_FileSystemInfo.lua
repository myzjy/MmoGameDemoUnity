---@class CS.System.IO.FileSystemInfo : CS.System.MarshalByRefObject
---@field public FullName string
---@field public Extension string
---@field public Name string
---@field public Exists boolean
---@field public CreationTime CS.System.DateTime
---@field public CreationTimeUtc CS.System.DateTime
---@field public LastAccessTime CS.System.DateTime
---@field public LastAccessTimeUtc CS.System.DateTime
---@field public LastWriteTime CS.System.DateTime
---@field public LastWriteTimeUtc CS.System.DateTime
---@field public Attributes number
CS.System.IO.FileSystemInfo = { }
function CS.System.IO.FileSystemInfo:Delete() end
function CS.System.IO.FileSystemInfo:Refresh() end
---@param info CS.System.Runtime.Serialization.SerializationInfo
---@param context CS.System.Runtime.Serialization.StreamingContext
function CS.System.IO.FileSystemInfo:GetObjectData(info, context) end
return CS.System.IO.FileSystemInfo
