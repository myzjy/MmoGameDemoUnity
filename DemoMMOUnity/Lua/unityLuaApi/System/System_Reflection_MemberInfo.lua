---@class System.Reflection.MemberInfo
---@field public MemberType number
---@field public Name string
---@field public DeclaringType string
---@field public ReflectedType string
---@field public CustomAttributes System.Collections.Generic.IEnumerable_System.Reflection.CustomAttributeData
---@field public MetadataToken number
---@field public Module System.Reflection.Module

---@type System.Reflection.MemberInfo
System.Reflection.MemberInfo = { }
---@overload fun(inherit:boolean): System.Object[]
---@return System.Object[]
---@param attributeType string
---@param inherit boolean
function System.Reflection.MemberInfo:GetCustomAttributes(attributeType, inherit) end
---@return boolean
---@param attributeType string
---@param inherit boolean
function System.Reflection.MemberInfo:IsDefined(attributeType, inherit) end
---@return System.Collections.Generic.IList_System.Reflection.CustomAttributeData
function System.Reflection.MemberInfo:GetCustomAttributesData() end
---@return boolean
---@param left System.Reflection.MemberInfo
---@param right System.Reflection.MemberInfo
function System.Reflection.MemberInfo.op_Equality(left, right) end
---@return boolean
---@param left System.Reflection.MemberInfo
---@param right System.Reflection.MemberInfo
function System.Reflection.MemberInfo.op_Inequality(left, right) end
---@return boolean
---@param obj System.Object
function System.Reflection.MemberInfo:Equals(obj) end
---@return number
function System.Reflection.MemberInfo:GetHashCode() end
return System.Reflection.MemberInfo
