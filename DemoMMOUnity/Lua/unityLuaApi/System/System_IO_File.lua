---@class System.IO.File

---@type System.IO.File
System.IO.File = { }
---@overload fun(path:string, contents:string): void
---@param path string
---@param contents string
---@param encoding System.Text.Encoding
function System.IO.File.AppendAllText(path, contents, encoding) end
---@return System.IO.StreamWriter
---@param path string
function System.IO.File.AppendText(path) end
---@overload fun(sourceFileName:string, destFileName:string): void
---@param sourceFileName string
---@param destFileName string
---@param overwrite boolean
function System.IO.File.Copy(sourceFileName, destFileName, overwrite) end
---@overload fun(path:string): System.IO.FileStream
---@overload fun(path:string, bufferSize:number): System.IO.FileStream
---@overload fun(path:string, bufferSize:number, options:number): System.IO.FileStream
---@return System.IO.FileStream
---@param path string
---@param bufferSize number
---@param options number
---@param fileSecurity System.Security.AccessControl.FileSecurity
function System.IO.File.Create(path, bufferSize, options, fileSecurity) end
---@return System.IO.StreamWriter
---@param path string
function System.IO.File.CreateText(path) end
---@param path string
function System.IO.File.Delete(path) end
---@return boolean
---@param path string
function System.IO.File.Exists(path) end
---@overload fun(path:string): System.Security.AccessControl.FileSecurity
---@return System.Security.AccessControl.FileSecurity
---@param path string
---@param includeSections number
function System.IO.File.GetAccessControl(path, includeSections) end
---@return number
---@param path string
function System.IO.File.GetAttributes(path) end
---@return System.DateTime
---@param path string
function System.IO.File.GetCreationTime(path) end
---@return System.DateTime
---@param path string
function System.IO.File.GetCreationTimeUtc(path) end
---@return System.DateTime
---@param path string
function System.IO.File.GetLastAccessTime(path) end
---@return System.DateTime
---@param path string
function System.IO.File.GetLastAccessTimeUtc(path) end
---@return System.DateTime
---@param path string
function System.IO.File.GetLastWriteTime(path) end
---@return System.DateTime
---@param path string
function System.IO.File.GetLastWriteTimeUtc(path) end
---@param sourceFileName string
---@param destFileName string
function System.IO.File.Move(sourceFileName, destFileName) end
---@overload fun(path:string, mode:number): System.IO.FileStream
---@overload fun(path:string, mode:number, access:number): System.IO.FileStream
---@return System.IO.FileStream
---@param path string
---@param mode number
---@param access number
---@param share number
function System.IO.File.Open(path, mode, access, share) end
---@return System.IO.FileStream
---@param path string
function System.IO.File.OpenRead(path) end
---@return System.IO.StreamReader
---@param path string
function System.IO.File.OpenText(path) end
---@return System.IO.FileStream
---@param path string
function System.IO.File.OpenWrite(path) end
---@overload fun(sourceFileName:string, destinationFileName:string, destinationBackupFileName:string): void
---@param sourceFileName string
---@param destinationFileName string
---@param destinationBackupFileName string
---@param ignoreMetadataErrors boolean
function System.IO.File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors) end
---@param path string
---@param fileSecurity System.Security.AccessControl.FileSecurity
function System.IO.File.SetAccessControl(path, fileSecurity) end
---@param path string
---@param fileAttributes number
function System.IO.File.SetAttributes(path, fileAttributes) end
---@param path string
---@param creationTime System.DateTime
function System.IO.File.SetCreationTime(path, creationTime) end
---@param path string
---@param creationTimeUtc System.DateTime
function System.IO.File.SetCreationTimeUtc(path, creationTimeUtc) end
---@param path string
---@param lastAccessTime System.DateTime
function System.IO.File.SetLastAccessTime(path, lastAccessTime) end
---@param path string
---@param lastAccessTimeUtc System.DateTime
function System.IO.File.SetLastAccessTimeUtc(path, lastAccessTimeUtc) end
---@param path string
---@param lastWriteTime System.DateTime
function System.IO.File.SetLastWriteTime(path, lastWriteTime) end
---@param path string
---@param lastWriteTimeUtc System.DateTime
function System.IO.File.SetLastWriteTimeUtc(path, lastWriteTimeUtc) end
---@return System.Byte[]
---@param path string
function System.IO.File.ReadAllBytes(path) end
---@overload fun(path:string): System.String[]
---@return System.String[]
---@param path string
---@param encoding System.Text.Encoding
function System.IO.File.ReadAllLines(path, encoding) end
---@overload fun(path:string): string
---@return string
---@param path string
---@param encoding System.Text.Encoding
function System.IO.File.ReadAllText(path, encoding) end
---@param path string
---@param bytes System.Byte[]
function System.IO.File.WriteAllBytes(path, bytes) end
---@overload fun(path:string, contents:System.String[]): void
---@overload fun(path:string, contents:System.Collections.Generic.IEnumerable_System.String): void
---@overload fun(path:string, contents:System.String[], encoding:System.Text.Encoding): void
---@param path string
---@param contents System.Collections.Generic.IEnumerable_System.String
---@param encoding System.Text.Encoding
function System.IO.File.WriteAllLines(path, contents, encoding) end
---@overload fun(path:string, contents:string): void
---@param path string
---@param contents string
---@param encoding System.Text.Encoding
function System.IO.File.WriteAllText(path, contents, encoding) end
---@param path string
function System.IO.File.Encrypt(path) end
---@param path string
function System.IO.File.Decrypt(path) end
---@overload fun(path:string): System.Collections.Generic.IEnumerable_System.String
---@return System.Collections.Generic.IEnumerable_System.String
---@param path string
---@param encoding System.Text.Encoding
function System.IO.File.ReadLines(path, encoding) end
---@overload fun(path:string, contents:System.Collections.Generic.IEnumerable_System.String): void
---@param path string
---@param contents System.Collections.Generic.IEnumerable_System.String
---@param encoding System.Text.Encoding
function System.IO.File.AppendAllLines(path, contents, encoding) end
return System.IO.File
