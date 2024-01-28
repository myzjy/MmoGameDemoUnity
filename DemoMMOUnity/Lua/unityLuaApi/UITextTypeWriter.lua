---@class UITextTypeWriter : UnityEngine.MonoBehaviour
---@field public CHANGELINE_TYPE number
---@field public totalTime number
---@field public waitTime number

---@type UITextTypeWriter
UITextTypeWriter = { }
---@return UITextTypeWriter
function UITextTypeWriter.New() end
function UITextTypeWriter:Release() end
function UITextTypeWriter:TestTypeWrite() end
---@param typeWriteText string
---@param setcharsPerSecond number
---@param setStayTime number
---@param callback (fun():void)
function UITextTypeWriter:PlayTypeWriterBySetcharsPerSecond(typeWriteText, setcharsPerSecond, setStayTime, callback) end
---@param typeWriteText string
---@param setTotalTime number
---@param setStayTime number
function UITextTypeWriter:PlayTypeWriter(typeWriteText, setTotalTime, setStayTime) end
---@param typeWriteText string
---@param setTotalTime number
---@param setStayTime number
function UITextTypeWriter:InitTypeWriteInfo(typeWriteText, setTotalTime, setStayTime) end
---@return number
function UITextTypeWriter:GetTypeWriterTotalTime() end
return UITextTypeWriter
