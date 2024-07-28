---@class CS.System.IO.File
CS.System.IO.File = { }
---@overload fun(path:string, contents:string): void
---@param path string
---@param contents string
---@param encoding CS.System.Text.Encoding
function CS.System.IO.File.AppendAllText(path, contents, encoding) end
---@return CS.System.IO.StreamWriter
---@param path string
function CS.System.IO.File.AppendText(path) end
---@overload fun(sourceFileName:string, destFileName:string): void
---@param sourceFileName string
---@param destFileName string
---@param overwrite boolean
function CS.System.IO.File.Copy(sourceFileName, destFileName, overwrite) end
---@overload fun(path:string): CS.System.IO.FileStream
---@overload fun(path:string, bufferSize:number): CS.System.IO.FileStream
---@overload fun(path:string, bufferSize:number, options:number): CS.System.IO.FileStream
---@return CS.System.IO.FileStream
---@param path string
---@param bufferSize number
---@param options number
---@param fileSecurity CS.System.Security.AccessControl.FileSecurity
function CS.System.IO.File.Create(path, bufferSize, options, fileSecurity) end
---@return CS.System.IO.StreamWriter
---@param path string
function CS.System.IO.File.CreateText(path) end
---@param path string
function CS.System.IO.File.Delete(path) end
---@return boolean
---@param path string
function CS.System.IO.File.Exists(path) end
---@overload fun(path:string): CS.System.Security.AccessControl.FileSecurity
---@return CS.System.Security.AccessControl.FileSecurity
---@param path string
---@param includeSections number
function CS.System.IO.File.GetAccessControl(path, includeSections) end
---@return number
---@param path string
function CS.System.IO.File.GetAttributes(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.File.GetCreationTime(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.File.GetCreationTimeUtc(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.File.GetLastAccessTime(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.File.GetLastAccessTimeUtc(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.File.GetLastWriteTime(path) end
---@return CS.System.DateTime
---@param path string
function CS.System.IO.File.GetLastWriteTimeUtc(path) end
---@param sourceFileName string
---@param destFileName string
function CS.System.IO.File.Move(sourceFileName, destFileName) end
---@overload fun(path:string, mode:number): CS.System.IO.FileStream
---@overload fun(path:string, mode:number, access:number): CS.System.IO.FileStream
---@return CS.System.IO.FileStream
---@param path string
---@param mode number
---@param access number
---@param share number
function CS.System.IO.File.Open(path, mode, access, share) end
---@return CS.System.IO.FileStream
---@param path string
function CS.System.IO.File.OpenRead(path) end
---@return CS.System.IO.StreamReader
---@param path string
function CS.System.IO.File.OpenText(path) end
---@return CS.System.IO.FileStream
---@param path string
function CS.System.IO.File.OpenWrite(path) end
---@overload fun(sourceFileName:string, destinationFileName:string, destinationBackupFileName:string): void
---@param sourceFileName string
---@param destinationFileName string
---@param destinationBackupFileName string
---@param ignoreMetadataErrors boolean
function CS.System.IO.File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors) end
---@param path string
---@param fileSecurity CS.System.Security.AccessControl.FileSecurity
function CS.System.IO.File.SetAccessControl(path, fileSecurity) end
---@param path string
---@param fileAttributes number
function CS.System.IO.File.SetAttributes(path, fileAttributes) end
---@param path string
---@param creationTime CS.System.DateTime
function CS.System.IO.File.SetCreationTime(path, creationTime) end
---@param path string
---@param creationTimeUtc CS.System.DateTime
function CS.System.IO.File.SetCreationTimeUtc(path, creationTimeUtc) end
---@param path string
---@param lastAccessTime CS.System.DateTime
function CS.System.IO.File.SetLastAccessTime(path, lastAccessTime) end
---@param path string
---@param lastAccessTimeUtc CS.System.DateTime
function CS.System.IO.File.SetLastAccessTimeUtc(path, lastAccessTimeUtc) end
---@param path string
---@param lastWriteTime CS.System.DateTime
function CS.System.IO.File.SetLastWriteTime(path, lastWriteTime) end
---@param path string
---@param lastWriteTimeUtc CS.System.DateTime
function CS.System.IO.File.SetLastWriteTimeUtc(path, lastWriteTimeUtc) end
---@return CS.System.Byte[]
---@param path string
function CS.System.IO.File.ReadAllBytes(path) end
---@overload fun(path:string): CS.System.String[]
---@return CS.System.String[]
---@param path string
---@param encoding CS.System.Text.Encoding
function CS.System.IO.File.ReadAllLines(path, encoding) end
---@overload fun(path:string): string
---@return string
---@param path string
---@param encoding CS.System.Text.Encoding
function CS.System.IO.File.ReadAllText(path, encoding) end
---@param path string
---@param bytes CS.System.Byte[]
function CS.System.IO.File.WriteAllBytes(path, bytes) end
---@overload fun(path:string, contents:CS.System.String[]): void
---@overload fun(path:string, contents:CS.System.Collections.Generic.IEnumerable_System.String): void
---@overload fun(path:string, contents:CS.System.String[], encoding:CS.System.Text.Encoding): void
---@param path string
---@param contents CS.System.Collections.Generic.IEnumerable_System.String
---@param encoding CS.System.Text.Encoding
function CS.System.IO.File.WriteAllLines(path, contents, encoding) end
---@overload fun(path:string, contents:string): void
---@param path string
---@param contents string
---@param encoding CS.System.Text.Encoding
function CS.System.IO.File.WriteAllText(path, contents, encoding) end
---@param path string
function CS.System.IO.File.Encrypt(path) end
---@param path string
function CS.System.IO.File.Decrypt(path) end
---@overload fun(path:string): CS.System.Collections.Generic.IEnumerable_System.String
---@return CS.System.Collections.Generic.IEnumerable_System.String
---@param path string
---@param encoding CS.System.Text.Encoding
function CS.System.IO.File.ReadLines(path, encoding) end
---@overload fun(path:string, contents:CS.System.Collections.Generic.IEnumerable_System.String): void
---@param path string
---@param contents CS.System.Collections.Generic.IEnumerable_System.String
---@param encoding CS.System.Text.Encoding
function CS.System.IO.File.AppendAllLines(path, contents, encoding) end
return CS.System.IO.File
