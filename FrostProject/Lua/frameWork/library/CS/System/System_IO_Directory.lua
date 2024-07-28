---@class CS.System.IO.Directory
CS.System.IO.Directory = { }
---@overload fun(path:string): CS.System.String[]
---@overload fun(path:string, searchPattern:string): CS.System.String[]
---@return CS.System.String[]
---@param path string
---@param searchPattern string
---@param searchOption number
function CS.System.IO.Directory.GetFiles(path, searchPattern, searchOption) end
---@overload fun(path:string): CS.System.String[]
---@overload fun(path:string, searchPattern:string): CS.System.String[]
---@return CS.System.String[]
---@param path string
---@param searchPattern string
---@param searchOption number
function CS.System.IO.Directory.GetDirectories(path, searchPattern, searchOption) end
---@overload fun(path:string): CS.System.String[]
---@overload fun(path:string, searchPattern:string): CS.System.String[]
---@return CS.System.String[]
---@param path string
---@param searchPattern string
---@param searchOption number
function CS.System.IO.Directory.GetFileSystemEntries(path, searchPattern, searchOption) end
---@overload fun(path:string): CS.System.Collections.Generic.IEnumerable_System.String
---@overload fun(path:string, searchPattern:string): CS.System.Collections.Generic.IEnumerable_System.String
---@return CS.System.Collections.Generic.IEnumerable_System.String
---@param path string
---@param searchPattern string
---@param searchOption number
function CS.System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOption) end
---@overload fun(path:string): CS.System.Collections.Generic.IEnumerable_System.String
---@overload fun(path:string, searchPattern:string): CS.System.Collections.Generic.IEnumerable_System.String
---@return CS.System.Collections.Generic.IEnumerable_System.String
---@param path string
---@param searchPattern string
---@param searchOption number
function CS.System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption) end
---@overload fun(path:string): CS.System.Collections.Generic.IEnumerable_System.String
---@overload fun(path:string, searchPattern:string): CS.System.Collections.Generic.IEnumerable_System.String
---@return CS.System.Collections.Generic.IEnumerable_System.String
---@param path string
---@param searchPattern string
---@param searchOption number
function CS.System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption) end
---@return string
---@param path string
function CS.System.IO.Directory.GetDirectoryRoot(path) end
---@overload fun(path:string): CS.System.IO.DirectoryInfo
---@return CS.System.IO.DirectoryInfo
---@param path string
---@param directorySecurity CS.System.Security.AccessControl.DirectorySecurity
function CS.System.IO.Directory.CreateDirectory(path, directorySecurity) end
---@overload fun(path:string): void
---@param path string
---@param recursive boolean
function CS.System.IO.Directory.Delete(path, recursive) end
---@return boolean
---@param path string
function CS.System.IO.Directory.Exists(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.Directory.GetLastAccessTime(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.Directory.GetLastAccessTimeUtc(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.Directory.GetLastWriteTime(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.Directory.GetLastWriteTimeUtc(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.Directory.GetCreationTime(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.Directory.GetCreationTimeUtc(path) end
---@return string
function CS.System.IO.Directory.GetCurrentDirectory() end
---@return CS.System.String[]
function CS.System.IO.Directory.GetLogicalDrives() end
---@return CS.System.IO.DirectoryInfo
---@param path string
function CS.System.IO.Directory.GetParent(path) end
---@param sourceDirName string
---@param destDirName string
function CS.System.IO.Directory.Move(sourceDirName, destDirName) end
---@param path string
---@param directorySecurity CS.System.Security.AccessControl.DirectorySecurity
function CS.System.IO.Directory.SetAccessControl(path, directorySecurity) end
---@param path string
---@param creationTime CS.System.DateTime
function CS.System.IO.Directory.SetCreationTime(path, creationTime) end
---@param path string
---@param creationTimeUtc CS.System.DateTime
function CS.System.IO.Directory.SetCreationTimeUtc(path, creationTimeUtc) end
---@param path string
function CS.System.IO.Directory.SetCurrentDirectory(path) end
---@param path string
---@param lastAccessTime CS.System.DateTime
function CS.System.IO.Directory.SetLastAccessTime(path, lastAccessTime) end
---@param path string
---@param lastAccessTimeUtc CS.System.DateTime
function CS.System.IO.Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc) end
---@param path string
---@param lastWriteTime CS.System.DateTime
function CS.System.IO.Directory.SetLastWriteTime(path, lastWriteTime) end
---@param path string
---@param lastWriteTimeUtc CS.System.DateTime
function CS.System.IO.Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc) end
---@overload fun(path:string): CS.System.Security.AccessControl.DirectorySecurity
---@return CS.System.Security.AccessControl.DirectorySecurity
---@param path string
---@param includeSections number
function CS.System.IO.Directory.GetAccessControl(path, includeSections) end
return CS.System.IO.Directory
