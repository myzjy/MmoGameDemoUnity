---@class System.IO.Path
---@field public AltDirectorySeparatorChar number
---@field public DirectorySeparatorChar number
---@field public PathSeparator number
---@field public VolumeSeparatorChar number

---@type System.IO.Path
System.IO.Path = { }
---@return string
---@param path string
---@param extension string
function System.IO.Path.ChangeExtension(path, extension) end
---@overload fun(paths:System.String[]): string
---@overload fun(path1:string, path2:string): string
---@overload fun(path1:string, path2:string, path3:string): string
---@return string
---@param path1 string
---@param path2 string
---@param path3 string
---@param path4 string
function System.IO.Path.Combine(path1, path2, path3, path4) end
---@return string
---@param path string
function System.IO.Path.GetDirectoryName(path) end
---@return string
---@param path string
function System.IO.Path.GetExtension(path) end
---@return string
---@param path string
function System.IO.Path.GetFileName(path) end
---@return string
---@param path string
function System.IO.Path.GetFileNameWithoutExtension(path) end
---@return string
---@param path string
function System.IO.Path.GetFullPath(path) end
---@return string
---@param path string
function System.IO.Path.GetPathRoot(path) end
---@return string
function System.IO.Path.GetTempFileName() end
---@return string
function System.IO.Path.GetTempPath() end
---@return boolean
---@param path string
function System.IO.Path.HasExtension(path) end
---@return boolean
---@param path string
function System.IO.Path.IsPathRooted(path) end
---@return System.Char[]
function System.IO.Path.GetInvalidFileNameChars() end
---@return System.Char[]
function System.IO.Path.GetInvalidPathChars() end
---@return string
function System.IO.Path.GetRandomFileName() end
return System.IO.Path
