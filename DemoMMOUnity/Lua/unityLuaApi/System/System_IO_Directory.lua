---@class System.IO.Directory

---@type System.IO.Directory
System.IO.Directory = { }
---@overload fun(path:string): System.String[]
---@overload fun(path:string, searchPattern:string): System.String[]
---@return System.String[]
---@param path string
---@param searchPattern string
---@param searchOption number
function System.IO.Directory.GetFiles(path, searchPattern, searchOption) end
---@overload fun(path:string): System.String[]
---@overload fun(path:string, searchPattern:string): System.String[]
---@return System.String[]
---@param path string
---@param searchPattern string
---@param searchOption number
function System.IO.Directory.GetDirectories(path, searchPattern, searchOption) end
---@overload fun(path:string): System.String[]
---@overload fun(path:string, searchPattern:string): System.String[]
---@return System.String[]
---@param path string
---@param searchPattern string
---@param searchOption number
function System.IO.Directory.GetFileSystemEntries(path, searchPattern, searchOption) end
---@overload fun(path:string): System.Collections.Generic.IEnumerable_System.String
---@overload fun(path:string, searchPattern:string): System.Collections.Generic.IEnumerable_System.String
---@return System.Collections.Generic.IEnumerable_System.String
---@param path string
---@param searchPattern string
---@param searchOption number
function System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOption) end
---@overload fun(path:string): System.Collections.Generic.IEnumerable_System.String
---@overload fun(path:string, searchPattern:string): System.Collections.Generic.IEnumerable_System.String
---@return System.Collections.Generic.IEnumerable_System.String
---@param path string
---@param searchPattern string
---@param searchOption number
function System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption) end
---@overload fun(path:string): System.Collections.Generic.IEnumerable_System.String
---@overload fun(path:string, searchPattern:string): System.Collections.Generic.IEnumerable_System.String
---@return System.Collections.Generic.IEnumerable_System.String
---@param path string
---@param searchPattern string
---@param searchOption number
function System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption) end
---@return string
---@param path string
function System.IO.Directory.GetDirectoryRoot(path) end
---@overload fun(path:string): System.IO.DirectoryInfo
---@return System.IO.DirectoryInfo
---@param path string
---@param directorySecurity System.Security.AccessControl.DirectorySecurity
function System.IO.Directory.CreateDirectory(path, directorySecurity) end
---@overload fun(path:string): void
---@param path string
---@param recursive boolean
function System.IO.Directory.Delete(path, recursive) end
---@return boolean
---@param path string
function System.IO.Directory.Exists(path) end
---@return System.DateTime
---@param path string
function System.IO.Directory.GetLastAccessTime(path) end
---@return System.DateTime
---@param path string
function System.IO.Directory.GetLastAccessTimeUtc(path) end
---@return System.DateTime
---@param path string
function System.IO.Directory.GetLastWriteTime(path) end
---@return System.DateTime
---@param path string
function System.IO.Directory.GetLastWriteTimeUtc(path) end
---@return System.DateTime
---@param path string
function System.IO.Directory.GetCreationTime(path) end
---@return System.DateTime
---@param path string
function System.IO.Directory.GetCreationTimeUtc(path) end
---@return string
function System.IO.Directory.GetCurrentDirectory() end
---@return System.String[]
function System.IO.Directory.GetLogicalDrives() end
---@return System.IO.DirectoryInfo
---@param path string
function System.IO.Directory.GetParent(path) end
---@param sourceDirName string
---@param destDirName string
function System.IO.Directory.Move(sourceDirName, destDirName) end
---@param path string
---@param directorySecurity System.Security.AccessControl.DirectorySecurity
function System.IO.Directory.SetAccessControl(path, directorySecurity) end
---@param path string
---@param creationTime System.DateTime
function System.IO.Directory.SetCreationTime(path, creationTime) end
---@param path string
---@param creationTimeUtc System.DateTime
function System.IO.Directory.SetCreationTimeUtc(path, creationTimeUtc) end
---@param path string
function System.IO.Directory.SetCurrentDirectory(path) end
---@param path string
---@param lastAccessTime System.DateTime
function System.IO.Directory.SetLastAccessTime(path, lastAccessTime) end
---@param path string
---@param lastAccessTimeUtc System.DateTime
function System.IO.Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc) end
---@param path string
---@param lastWriteTime System.DateTime
function System.IO.Directory.SetLastWriteTime(path, lastWriteTime) end
---@param path string
---@param lastWriteTimeUtc System.DateTime
function System.IO.Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc) end
---@overload fun(path:string): System.Security.AccessControl.DirectorySecurity
---@return System.Security.AccessControl.DirectorySecurity
---@param path string
---@param includeSections number
function System.IO.Directory.GetAccessControl(path, includeSections) end
return System.IO.Directory
