---@class CS.System.Reflection.MemberInfo
---@field public MemberType number
---@field public Name string
---@field public DeclaringType string
---@field public ReflectedType string
---@field public CustomAttributes CS.System.Collections.Generic.IEnumerable_System.Reflection.CustomAttributeData
---@field public MetadataToken number
---@field public Module CS.System.Reflection.Module
CS.System.Reflection.MemberInfo = { }
---@overload fun(inherit:boolean): CS.System.Object[]
---@return CS.System.Object[]
---@param attributeType string
---@param inherit boolean
function CS.System.Reflection.MemberInfo:GetCustomAttributes(attributeType, inherit) end
---@return boolean
---@param attributeType string
---@param inherit boolean
function CS.System.Reflection.MemberInfo:IsDefined(attributeType, inherit) end
---@return CS.System.Collections.Generic.IList_System.Reflection.CustomAttributeData
function CS.System.Reflection.MemberInfo:GetCustomAttributesData() end
---@return boolean
---@param left CS.System.Reflection.MemberInfo
---@param right CS.System.Reflection.MemberInfo
function CS.System.Reflection.MemberInfo.op_Equality(left, right) end
---@return boolean
---@param left CS.System.Reflection.MemberInfo
---@param right CS.System.Reflection.MemberInfo
function CS.System.Reflection.MemberInfo.op_Inequality(left, right) end
---@return boolean
---@param obj CS.System.Object
function CS.System.Reflection.MemberInfo:Equals(obj) end
---@return number
function CS.System.Reflection.MemberInfo:GetHashCode() end
return CS.System.Reflection.MemberInfo
