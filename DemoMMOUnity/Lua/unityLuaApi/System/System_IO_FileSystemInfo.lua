---@class System.IO.FileSystemInfo : System.MarshalByRefObject
---@field public FullName string
---@field public Extension string
---@field public Name string
---@field public Exists boolean
---@field public CreationTime System.DateTime
---@field public CreationTimeUtc System.DateTime
---@field public LastAccessTime System.DateTime
---@field public LastAccessTimeUtc System.DateTime
---@field public LastWriteTime System.DateTime
---@field public LastWriteTimeUtc System.DateTime
---@field public Attributes number

---@type System.IO.FileSystemInfo
System.IO.FileSystemInfo = { }
function System.IO.FileSystemInfo:Delete() end
function System.IO.FileSystemInfo:Refresh() end
---@param info System.Runtime.Serialization.SerializationInfo
---@param context System.Runtime.Serialization.StreamingContext
function System.IO.FileSystemInfo:GetObjectData(info, context) end
return System.IO.FileSystemInfo
