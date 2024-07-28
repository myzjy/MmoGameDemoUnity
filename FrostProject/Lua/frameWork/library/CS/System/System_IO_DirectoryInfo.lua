---@class CS.System.IO.DirectoryInfo : CS.System.IO.FileSystemInfo
---@field public Exists boolean
---@field public Name string
---@field public Parent CS.System.IO.DirectoryInfo
---@field public Root CS.System.IO.DirectoryInfo
CS.System.IO.DirectoryInfo = { }
---@return CS.System.IO.DirectoryInfo
---@param path string
function CS.System.IO.DirectoryInfo.New(path) end
---@overload fun(): void
---@param directorySecurity CS.System.Security.AccessControl.DirectorySecurity
function CS.System.IO.DirectoryInfo:Create(directorySecurity) end
---@overload fun(path:string): CS.System.IO.DirectoryInfo
---@return CS.System.IO.DirectoryInfo
---@param path string
---@param directorySecurity CS.System.Security.AccessControl.DirectorySecurity
function CS.System.IO.DirectoryInfo:CreateSubdirectory(path, directorySecurity) end
---@overload fun(): CS.System.IO.FileInfo[]
---@overload fun(searchPattern:string): CS.System.IO.FileInfo[]
---@return CS.System.IO.FileInfo[]
---@param searchPattern string
---@param searchOption number
function CS.System.IO.DirectoryInfo:GetFiles(searchPattern, searchOption) end
---@overload fun(): CS.System.IO.DirectoryInfo[]
---@overload fun(searchPattern:string): CS.System.IO.DirectoryInfo[]
---@return CS.System.IO.DirectoryInfo[]
---@param searchPattern string
---@param searchOption number
function CS.System.IO.DirectoryInfo:GetDirectories(searchPattern, searchOption) end
---@overload fun(): CS.System.IO.FileSystemInfo[]
---@overload fun(searchPattern:string): CS.System.IO.FileSystemInfo[]
---@return CS.System.IO.FileSystemInfo[]
---@param searchPattern string
---@param searchOption number
function CS.System.IO.DirectoryInfo:GetFileSystemInfos(searchPattern, searchOption) end
---@overload fun(): void
---@param recursive boolean
function CS.System.IO.DirectoryInfo:Delete(recursive) end
---@param destDirName string
function CS.System.IO.DirectoryInfo:MoveTo(destDirName) end
---@return string
function CS.System.IO.DirectoryInfo:ToString() end
---@overload fun(): CS.System.Security.AccessControl.DirectorySecurity
---@return CS.System.Security.AccessControl.DirectorySecurity
---@param includeSections number
function CS.System.IO.DirectoryInfo:GetAccessControl(includeSections) end
---@param directorySecurity CS.System.Security.AccessControl.DirectorySecurity
function CS.System.IO.DirectoryInfo:SetAccessControl(directorySecurity) end
---@overload fun(): CS.System.Collections.Generic.IEnumerable_System.IO.DirectoryInfo
---@overload fun(searchPattern:string): CS.System.Collections.Generic.IEnumerable_System.IO.DirectoryInfo
---@return CS.System.Collections.Generic.IEnumerable_System.IO.DirectoryInfo
---@param searchPattern string
---@param searchOption number
function CS.System.IO.DirectoryInfo:EnumerateDirectories(searchPattern, searchOption) end
---@overload fun(): CS.System.Collections.Generic.IEnumerable_System.IO.FileInfo
---@overload fun(searchPattern:string): CS.System.Collections.Generic.IEnumerable_System.IO.FileInfo
---@return CS.System.Collections.Generic.IEnumerable_System.IO.FileInfo
---@param searchPattern string
---@param searchOption number
function CS.System.IO.DirectoryInfo:EnumerateFiles(searchPattern, searchOption) end
---@overload fun(): CS.System.Collections.Generic.IEnumerable_System.IO.FileSystemInfo
---@overload fun(searchPattern:string): CS.System.Collections.Generic.IEnumerable_System.IO.FileSystemInfo
---@return CS.System.Collections.Generic.IEnumerable_System.IO.FileSystemInfo
---@param searchPattern string
---@param searchOption number
function CS.System.IO.DirectoryInfo:EnumerateFileSystemInfos(searchPattern, searchOption) end
return CS.System.IO.DirectoryInfo
