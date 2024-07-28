---@class CS.System.Type : CS.System.Reflection.MemberInfo
---@field public FilterAttribute (fun(m:CS.System.Reflection.MemberInfo, filterCriteria:CS.System.Object):boolean)
---@field public FilterName (fun(m:CS.System.Reflection.MemberInfo, filterCriteria:CS.System.Object):boolean)
---@field public FilterNameIgnoreCase (fun(m:CS.System.Reflection.MemberInfo, filterCriteria:CS.System.Object):boolean)
---@field public Missing CS.System.Object
---@field public Delimiter number
---@field public EmptyTypes CS.System.Type[]
---@field public MemberType number
---@field public DeclaringType string
---@field public DeclaringMethod CS.System.Reflection.MethodBase
---@field public ReflectedType string
---@field public StructLayoutAttribute CS.System.Runtime.InteropServices.StructLayoutAttribute
---@field public GUID CS.System.Guid
---@field public DefaultBinder CS.System.Reflection.Binder
---@field public Module CS.System.Reflection.Module
---@field public Assembly CS.System.Reflection.Assembly
---@field public TypeHandle CS.System.RuntimeTypeHandle
---@field public FullName string
---@field public Namespace string
---@field public AssemblyQualifiedName string
---@field public BaseType string
---@field public TypeInitializer CS.System.Reflection.ConstructorInfo
---@field public IsNested boolean
---@field public Attributes number
---@field public GenericParameterAttributes number
---@field public IsVisible boolean
---@field public IsNotPublic boolean
---@field public IsPublic boolean
---@field public IsNestedPublic boolean
---@field public IsNestedPrivate boolean
---@field public IsNestedFamily boolean
---@field public IsNestedAssembly boolean
---@field public IsNestedFamANDAssem boolean
---@field public IsNestedFamORAssem boolean
---@field public IsAutoLayout boolean
---@field public IsLayoutSequential boolean
---@field public IsExplicitLayout boolean
---@field public IsClass boolean
---@field public IsInterface boolean
---@field public IsValueType boolean
---@field public IsAbstract boolean
---@field public IsSealed boolean
---@field public IsEnum boolean
---@field public IsSpecialName boolean
---@field public IsImport boolean
---@field public IsSerializable boolean
---@field public IsAnsiClass boolean
---@field public IsUnicodeClass boolean
---@field public IsAutoClass boolean
---@field public IsArray boolean
---@field public IsGenericType boolean
---@field public IsGenericTypeDefinition boolean
---@field public IsConstructedGenericType boolean
---@field public IsGenericParameter boolean
---@field public GenericParameterPosition number
---@field public ContainsGenericParameters boolean
---@field public IsByRef boolean
---@field public IsPointer boolean
---@field public IsPrimitive boolean
---@field public IsCOMObject boolean
---@field public HasElementType boolean
---@field public IsContextful boolean
---@field public IsMarshalByRef boolean
---@field public GenericTypeArguments CS.System.Type[]
---@field public IsSecurityCritical boolean
---@field public IsSecuritySafeCritical boolean
---@field public IsSecurityTransparent boolean
---@field public UnderlyingSystemType string
---@field public IsSZArray boolean
CS.System.Type = { }
---@overload fun(): string
---@overload fun(typeName:string): string
---@overload fun(typeName:string, throwOnError:boolean): string
---@overload fun(typeName:string, assemblyResolver:(fun(arg:CS.System.Reflection.AssemblyName):CS.System.Reflection.Assembly), typeResolver:(fun(arg1:CS.System.Reflection.Assembly, arg2:string, arg3:boolean):string)): string
---@overload fun(typeName:string, throwOnError:boolean, ignoreCase:boolean): string
---@overload fun(typeName:string, assemblyResolver:(fun(arg:CS.System.Reflection.AssemblyName):CS.System.Reflection.Assembly), typeResolver:(fun(arg1:CS.System.Reflection.Assembly, arg2:string, arg3:boolean):string), throwOnError:boolean): string
---@return string
---@param typeName string
---@param assemblyResolver (fun(arg:CS.System.Reflection.AssemblyName):CS.System.Reflection.Assembly)
---@param typeResolver (fun(arg1:CS.System.Reflection.Assembly, arg2:string, arg3:boolean):string)
---@param throwOnError boolean
---@param ignoreCase boolean
function CS.System.Type:GetType(typeName, assemblyResolver, typeResolver, throwOnError, ignoreCase) end
---@return string
function CS.System.Type:MakePointerType() end
---@return string
function CS.System.Type:MakeByRefType() end
---@overload fun(): string
---@return string
---@param rank number
function CS.System.Type:MakeArrayType(rank) end
---@overload fun(progID:string): string
---@overload fun(progID:string, throwOnError:boolean): string
---@overload fun(progID:string, server:string): string
---@return string
---@param progID string
---@param server string
---@param throwOnError boolean
function CS.System.Type.GetTypeFromProgID(progID, server, throwOnError) end
---@overload fun(clsid:CS.System.Guid): string
---@overload fun(clsid:CS.System.Guid, throwOnError:boolean): string
---@overload fun(clsid:CS.System.Guid, server:string): string
---@return string
---@param clsid CS.System.Guid
---@param server string
---@param throwOnError boolean
function CS.System.Type.GetTypeFromCLSID(clsid, server, throwOnError) end
---@return number
---@param t string
function CS.System.Type.GetTypeCode(t) end
---@overload fun(name:string, invokeAttr:number, binder:CS.System.Reflection.Binder, target:CS.System.Object, args:CS.System.Object[]): CS.System.Object
---@overload fun(name:string, invokeAttr:number, binder:CS.System.Reflection.Binder, target:CS.System.Object, args:CS.System.Object[], culture:CS.System.Globalization.CultureInfo): CS.System.Object
---@return CS.System.Object
---@param name string
---@param invokeAttr number
---@param binder CS.System.Reflection.Binder
---@param target CS.System.Object
---@param args CS.System.Object[]
---@param modifiers CS.System.Reflection.ParameterModifier[]
---@param culture CS.System.Globalization.CultureInfo
---@param namedParameters CS.System.String[]
function CS.System.Type:InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters) end
---@return CS.System.RuntimeTypeHandle
---@param o CS.System.Object
function CS.System.Type.GetTypeHandle(o) end
---@return number
function CS.System.Type:GetArrayRank() end
---@overload fun(types:CS.System.Type[]): CS.System.Reflection.ConstructorInfo
---@overload fun(bindingAttr:number, binder:CS.System.Reflection.Binder, types:CS.System.Type[], modifiers:CS.System.Reflection.ParameterModifier[]): CS.System.Reflection.ConstructorInfo
---@return CS.System.Reflection.ConstructorInfo
---@param bindingAttr number
---@param binder CS.System.Reflection.Binder
---@param callConvention number
---@param types CS.System.Type[]
---@param modifiers CS.System.Reflection.ParameterModifier[]
function CS.System.Type:GetConstructor(bindingAttr, binder, callConvention, types, modifiers) end
---@overload fun(): CS.System.Reflection.ConstructorInfo[]
---@return CS.System.Reflection.ConstructorInfo[]
---@param bindingAttr number
function CS.System.Type:GetConstructors(bindingAttr) end
---@overload fun(name:string): CS.System.Reflection.MethodInfo
---@overload fun(name:string, types:CS.System.Type[]): CS.System.Reflection.MethodInfo
---@overload fun(name:string, bindingAttr:number): CS.System.Reflection.MethodInfo
---@overload fun(name:string, types:CS.System.Type[], modifiers:CS.System.Reflection.ParameterModifier[]): CS.System.Reflection.MethodInfo
---@overload fun(name:string, bindingAttr:number, binder:CS.System.Reflection.Binder, types:CS.System.Type[], modifiers:CS.System.Reflection.ParameterModifier[]): CS.System.Reflection.MethodInfo
---@return CS.System.Reflection.MethodInfo
---@param name string
---@param bindingAttr number
---@param binder CS.System.Reflection.Binder
---@param callConvention number
---@param types CS.System.Type[]
---@param modifiers CS.System.Reflection.ParameterModifier[]
function CS.System.Type:GetMethod(name, bindingAttr, binder, callConvention, types, modifiers) end
---@overload fun(): CS.System.Reflection.MethodInfo[]
---@return CS.System.Reflection.MethodInfo[]
---@param bindingAttr number
function CS.System.Type:GetMethods(bindingAttr) end
---@overload fun(name:string): CS.System.Reflection.FieldInfo
---@return CS.System.Reflection.FieldInfo
---@param name string
---@param bindingAttr number
function CS.System.Type:GetField(name, bindingAttr) end
---@overload fun(): CS.System.Reflection.FieldInfo[]
---@return CS.System.Reflection.FieldInfo[]
---@param bindingAttr number
function CS.System.Type:GetFields(bindingAttr) end
---@overload fun(name:string): string
---@return string
---@param name string
---@param ignoreCase boolean
function CS.System.Type:GetInterface(name, ignoreCase) end
---@return CS.System.Type[]
function CS.System.Type:GetInterfaces() end
---@return CS.System.Type[]
---@param filter (fun(m:string, filterCriteria:CS.System.Object):boolean)
---@param filterCriteria CS.System.Object
function CS.System.Type:FindInterfaces(filter, filterCriteria) end
---@overload fun(name:string): CS.System.Reflection.EventInfo
---@return CS.System.Reflection.EventInfo
---@param name string
---@param bindingAttr number
function CS.System.Type:GetEvent(name, bindingAttr) end
---@overload fun(): CS.System.Reflection.EventInfo[]
---@return CS.System.Reflection.EventInfo[]
---@param bindingAttr number
function CS.System.Type:GetEvents(bindingAttr) end
---@overload fun(name:string): CS.System.Reflection.PropertyInfo
---@overload fun(name:string, bindingAttr:number): CS.System.Reflection.PropertyInfo
---@overload fun(name:string, types:CS.System.Type[]): CS.System.Reflection.PropertyInfo
---@overload fun(name:string, returnType:string): CS.System.Reflection.PropertyInfo
---@overload fun(name:string, returnType:string, types:CS.System.Type[]): CS.System.Reflection.PropertyInfo
---@overload fun(name:string, returnType:string, types:CS.System.Type[], modifiers:CS.System.Reflection.ParameterModifier[]): CS.System.Reflection.PropertyInfo
---@return CS.System.Reflection.PropertyInfo
---@param name string
---@param bindingAttr number
---@param binder CS.System.Reflection.Binder
---@param returnType string
---@param types CS.System.Type[]
---@param modifiers CS.System.Reflection.ParameterModifier[]
function CS.System.Type:GetProperty(name, bindingAttr, binder, returnType, types, modifiers) end
---@overload fun(): CS.System.Reflection.PropertyInfo[]
---@return CS.System.Reflection.PropertyInfo[]
---@param bindingAttr number
function CS.System.Type:GetProperties(bindingAttr) end
---@overload fun(): CS.System.Type[]
---@return CS.System.Type[]
---@param bindingAttr number
function CS.System.Type:GetNestedTypes(bindingAttr) end
---@overload fun(name:string): string
---@return string
---@param name string
---@param bindingAttr number
function CS.System.Type:GetNestedType(name, bindingAttr) end
---@overload fun(name:string): CS.System.Reflection.MemberInfo[]
---@overload fun(name:string, bindingAttr:number): CS.System.Reflection.MemberInfo[]
---@return CS.System.Reflection.MemberInfo[]
---@param name string
---@param t number
---@param bindingAttr number
function CS.System.Type:GetMember(name, t, bindingAttr) end
---@overload fun(): CS.System.Reflection.MemberInfo[]
---@return CS.System.Reflection.MemberInfo[]
---@param bindingAttr number
function CS.System.Type:GetMembers(bindingAttr) end
---@return CS.System.Reflection.MemberInfo[]
function CS.System.Type:GetDefaultMembers() end
---@return CS.System.Reflection.MemberInfo[]
---@param memberType number
---@param bindingAttr number
---@param filter (fun(m:CS.System.Reflection.MemberInfo, filterCriteria:CS.System.Object):boolean)
---@param filterCriteria CS.System.Object
function CS.System.Type:FindMembers(memberType, bindingAttr, filter, filterCriteria) end
---@return CS.System.Type[]
function CS.System.Type:GetGenericParameterConstraints() end
---@return string
---@param typeArguments CS.System.Type[]
function CS.System.Type:MakeGenericType(typeArguments) end
---@return string
function CS.System.Type:GetElementType() end
---@return CS.System.Type[]
function CS.System.Type:GetGenericArguments() end
---@return string
function CS.System.Type:GetGenericTypeDefinition() end
---@return CS.System.String[]
function CS.System.Type:GetEnumNames() end
---@return CS.System.Array
function CS.System.Type:GetEnumValues() end
---@return string
function CS.System.Type:GetEnumUnderlyingType() end
---@return boolean
---@param value CS.System.Object
function CS.System.Type:IsEnumDefined(value) end
---@return string
---@param value CS.System.Object
function CS.System.Type:GetEnumName(value) end
---@return boolean
---@param c string
function CS.System.Type:IsSubclassOf(c) end
---@return boolean
---@param o CS.System.Object
function CS.System.Type:IsInstanceOfType(o) end
---@return boolean
---@param c string
function CS.System.Type:IsAssignableFrom(c) end
---@return boolean
---@param other string
function CS.System.Type:IsEquivalentTo(other) end
---@return string
function CS.System.Type:ToString() end
---@return CS.System.Type[]
---@param args CS.System.Object[]
function CS.System.Type.GetTypeArray(args) end
---@overload fun(o:CS.System.Object): boolean
---@return boolean
---@param o string
function CS.System.Type:Equals(o) end
---@return boolean
---@param left string
---@param right string
function CS.System.Type.op_Equality(left, right) end
---@return boolean
---@param left string
---@param right string
function CS.System.Type.op_Inequality(left, right) end
---@return number
function CS.System.Type:GetHashCode() end
---@return CS.System.Reflection.InterfaceMapping
---@param interfaceType string
function CS.System.Type:GetInterfaceMap(interfaceType) end
---@return string
---@param typeName string
---@param throwIfNotFound boolean
---@param ignoreCase boolean
function CS.System.Type.ReflectionOnlyGetType(typeName, throwIfNotFound, ignoreCase) end
---@return string
---@param handle CS.System.RuntimeTypeHandle
function CS.System.Type.GetTypeFromHandle(handle) end
---@return boolean
function CS.System.Type:IsConvertableType() end
---@return boolean
function CS.System.Type:CanConvertFromString() end
---@return boolean
function CS.System.Type:CanConvertToString() end
return CS.System.Type
