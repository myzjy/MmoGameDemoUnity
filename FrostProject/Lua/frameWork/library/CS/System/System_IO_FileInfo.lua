---@class CS.System.IO.FileInfo : CS.System.IO.FileSystemInfo
---@field public Name string
---@field public Length int64
---@field public DirectoryName string
---@field public Directory CS.System.IO.DirectoryInfo
---@field public IsReadOnly boolean
---@field public Exists boolean
CS.System.IO.FileInfo = { }
---@return CS.System.IO.FileInfo
---@param fileName string
function CS.System.IO.FileInfo.New(fileName) end
---@overload fun(): CS.System.Security.AccessControl.FileSecurity
---@return CS.System.Security.AccessControl.FileSecurity
---@param includeSections number
function CS.System.IO.FileInfo:GetAccessControl(includeSections) end
---@param fileSecurity CS.System.Security.AccessControl.FileSecurity
function CS.System.IO.FileInfo:SetAccessControl(fileSecurity) end
---@return CS.System.IO.StreamReader
function CS.System.IO.FileInfo:OpenText() end
---@return CS.System.IO.StreamWriter
function CS.System.IO.FileInfo:CreateText() end
---@return CS.System.IO.StreamWriter
function CS.System.IO.FileInfo:AppendText() end
---@overload fun(destFileName:string): CS.System.IO.FileInfo
---@return CS.System.IO.FileInfo
---@param destFileName string
---@param overwrite boolean
function CS.System.IO.FileInfo:CopyTo(destFileName, overwrite) end
---@return CS.System.IO.FileStream
function CS.System.IO.FileInfo:Create() end
function CS.System.IO.FileInfo:Delete() end
function CS.System.IO.FileInfo:Decrypt() end
function CS.System.IO.FileInfo:Encrypt() end
---@overload fun(mode:number): CS.System.IO.FileStream
---@overload fun(mode:number, access:number): CS.System.IO.FileStream
---@return CS.System.IO.FileStream
---@param mode number
---@param access number
---@param share number
function CS.System.IO.FileInfo:Open(mode, access, share) end
---@return CS.System.IO.FileStream
function CS.System.IO.FileInfo:OpenRead() end
---@return CS.System.IO.FileStream
function CS.System.IO.FileInfo:OpenWrite() end
---@param destFileName string
function CS.System.IO.FileInfo:MoveTo(destFileName) end
---@overload fun(destinationFileName:string, destinationBackupFileName:string): CS.System.IO.FileInfo
---@return CS.System.IO.FileInfo
---@param destinationFileName string
---@param destinationBackupFileName string
---@param ignoreMetadataErrors boolean
function CS.System.IO.FileInfo:Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors) end
---@return string
function CS.System.IO.FileInfo:ToString() end
---@return string
---@param encoding CS.System.Text.Encoding
function CS.System.IO.FileInfo:ReadText(encoding) end
return CS.System.IO.FileInfo
