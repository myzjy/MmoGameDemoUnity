---@class System.IO.DirectoryInfo : System.IO.FileSystemInfo
---@field public Exists boolean
---@field public Name string
---@field public Parent System.IO.DirectoryInfo
---@field public Root System.IO.DirectoryInfo

---@type System.IO.DirectoryInfo
System.IO.DirectoryInfo = { }
---@return System.IO.DirectoryInfo
---@param path string
function System.IO.DirectoryInfo.New(path) end
---@overload fun(): void
---@param directorySecurity System.Security.AccessControl.DirectorySecurity
function System.IO.DirectoryInfo:Create(directorySecurity) end
---@overload fun(path:string): System.IO.DirectoryInfo
---@return System.IO.DirectoryInfo
---@param path string
---@param directorySecurity System.Security.AccessControl.DirectorySecurity
function System.IO.DirectoryInfo:CreateSubdirectory(path, directorySecurity) end
---@overload fun(): System.IO.FileInfo[]
---@overload fun(searchPattern:string): System.IO.FileInfo[]
---@return System.IO.FileInfo[]
---@param searchPattern string
---@param searchOption number
function System.IO.DirectoryInfo:GetFiles(searchPattern, searchOption) end
---@overload fun(): System.IO.DirectoryInfo[]
---@overload fun(searchPattern:string): System.IO.DirectoryInfo[]
---@return System.IO.DirectoryInfo[]
---@param searchPattern string
---@param searchOption number
function System.IO.DirectoryInfo:GetDirectories(searchPattern, searchOption) end
---@overload fun(): System.IO.FileSystemInfo[]
---@overload fun(searchPattern:string): System.IO.FileSystemInfo[]
---@return System.IO.FileSystemInfo[]
---@param searchPattern string
---@param searchOption number
function System.IO.DirectoryInfo:GetFileSystemInfos(searchPattern, searchOption) end
---@overload fun(): void
---@param recursive boolean
function System.IO.DirectoryInfo:Delete(recursive) end
---@param destDirName string
function System.IO.DirectoryInfo:MoveTo(destDirName) end
---@return string
function System.IO.DirectoryInfo:ToString() end
---@overload fun(): System.Security.AccessControl.DirectorySecurity
---@return System.Security.AccessControl.DirectorySecurity
---@param includeSections number
function System.IO.DirectoryInfo:GetAccessControl(includeSections) end
---@param directorySecurity System.Security.AccessControl.DirectorySecurity
function System.IO.DirectoryInfo:SetAccessControl(directorySecurity) end
---@overload fun(): System.Collections.Generic.IEnumerable_System.IO.DirectoryInfo
---@overload fun(searchPattern:string): System.Collections.Generic.IEnumerable_System.IO.DirectoryInfo
---@return System.Collections.Generic.IEnumerable_System.IO.DirectoryInfo
---@param searchPattern string
---@param searchOption number
function System.IO.DirectoryInfo:EnumerateDirectories(searchPattern, searchOption) end
---@overload fun(): System.Collections.Generic.IEnumerable_System.IO.FileInfo
---@overload fun(searchPattern:string): System.Collections.Generic.IEnumerable_System.IO.FileInfo
---@return System.Collections.Generic.IEnumerable_System.IO.FileInfo
---@param searchPattern string
---@param searchOption number
function System.IO.DirectoryInfo:EnumerateFiles(searchPattern, searchOption) end
---@overload fun(): System.Collections.Generic.IEnumerable_System.IO.FileSystemInfo
---@overload fun(searchPattern:string): System.Collections.Generic.IEnumerable_System.IO.FileSystemInfo
---@return System.Collections.Generic.IEnumerable_System.IO.FileSystemInfo
---@param searchPattern string
---@param searchOption number
function System.IO.DirectoryInfo:EnumerateFileSystemInfos(searchPattern, searchOption) end
return System.IO.DirectoryInfo
