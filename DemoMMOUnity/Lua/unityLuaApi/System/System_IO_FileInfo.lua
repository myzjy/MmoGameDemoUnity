---@class System.IO.FileInfo : System.IO.FileSystemInfo
---@field public Name string
---@field public Length int64
---@field public DirectoryName string
---@field public Directory System.IO.DirectoryInfo
---@field public IsReadOnly boolean
---@field public Exists boolean

---@type System.IO.FileInfo
System.IO.FileInfo = { }
---@return System.IO.FileInfo
---@param fileName string
function System.IO.FileInfo.New(fileName) end
---@overload fun(): System.Security.AccessControl.FileSecurity
---@return System.Security.AccessControl.FileSecurity
---@param includeSections number
function System.IO.FileInfo:GetAccessControl(includeSections) end
---@param fileSecurity System.Security.AccessControl.FileSecurity
function System.IO.FileInfo:SetAccessControl(fileSecurity) end
---@return System.IO.StreamReader
function System.IO.FileInfo:OpenText() end
---@return System.IO.StreamWriter
function System.IO.FileInfo:CreateText() end
---@return System.IO.StreamWriter
function System.IO.FileInfo:AppendText() end
---@overload fun(destFileName:string): System.IO.FileInfo
---@return System.IO.FileInfo
---@param destFileName string
---@param overwrite boolean
function System.IO.FileInfo:CopyTo(destFileName, overwrite) end
---@return System.IO.FileStream
function System.IO.FileInfo:Create() end
function System.IO.FileInfo:Delete() end
function System.IO.FileInfo:Decrypt() end
function System.IO.FileInfo:Encrypt() end
---@overload fun(mode:number): System.IO.FileStream
---@overload fun(mode:number, access:number): System.IO.FileStream
---@return System.IO.FileStream
---@param mode number
---@param access number
---@param share number
function System.IO.FileInfo:Open(mode, access, share) end
---@return System.IO.FileStream
function System.IO.FileInfo:OpenRead() end
---@return System.IO.FileStream
function System.IO.FileInfo:OpenWrite() end
---@param destFileName string
function System.IO.FileInfo:MoveTo(destFileName) end
---@overload fun(destinationFileName:string, destinationBackupFileName:string): System.IO.FileInfo
---@return System.IO.FileInfo
---@param destinationFileName string
---@param destinationBackupFileName string
---@param ignoreMetadataErrors boolean
function System.IO.FileInfo:Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors) end
---@return string
function System.IO.FileInfo:ToString() end
---@return string
---@param encoding System.Text.Encoding
function System.IO.FileInfo:ReadText(encoding) end
return System.IO.FileInfo
