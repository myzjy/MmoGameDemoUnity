---@class System.Type : System.Reflection.MemberInfo
---@field public FilterAttribute (fun(m:System.Reflection.MemberInfo, filterCriteria:System.Object):boolean)
---@field public FilterName (fun(m:System.Reflection.MemberInfo, filterCriteria:System.Object):boolean)
---@field public FilterNameIgnoreCase (fun(m:System.Reflection.MemberInfo, filterCriteria:System.Object):boolean)
---@field public Missing System.Object
---@field public Delimiter number
---@field public EmptyTypes System.Type[]
---@field public MemberType number
---@field public DeclaringType string
---@field public DeclaringMethod System.Reflection.MethodBase
---@field public ReflectedType string
---@field public StructLayoutAttribute System.Runtime.InteropServices.StructLayoutAttribute
---@field public GUID System.Guid
---@field public DefaultBinder System.Reflection.Binder
---@field public Module System.Reflection.Module
---@field public Assembly System.Reflection.Assembly
---@field public TypeHandle System.RuntimeTypeHandle
---@field public FullName string
---@field public Namespace string
---@field public AssemblyQualifiedName string
---@field public BaseType string
---@field public TypeInitializer System.Reflection.ConstructorInfo
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
---@field public GenericTypeArguments System.Type[]
---@field public IsSecurityCritical boolean
---@field public IsSecuritySafeCritical boolean
---@field public IsSecurityTransparent boolean
---@field public UnderlyingSystemType string
---@field public IsSZArray boolean

---@type System.Type
System.Type = { }
---@overload fun(): string
---@overload fun(typeName:string): string
---@overload fun(typeName:string, throwOnError:boolean): string
---@overload fun(typeName:string, assemblyResolver:(fun(arg:System.Reflection.AssemblyName):System.Reflection.Assembly), typeResolver:(fun(arg1:System.Reflection.Assembly, arg2:string, arg3:boolean):string)): string
---@overload fun(typeName:string, throwOnError:boolean, ignoreCase:boolean): string
---@overload fun(typeName:string, assemblyResolver:(fun(arg:System.Reflection.AssemblyName):System.Reflection.Assembly), typeResolver:(fun(arg1:System.Reflection.Assembly, arg2:string, arg3:boolean):string), throwOnError:boolean): string
---@return string
---@param typeName string
---@param assemblyResolver (fun(arg:System.Reflection.AssemblyName):System.Reflection.Assembly)
---@param typeResolver (fun(arg1:System.Reflection.Assembly, arg2:string, arg3:boolean):string)
---@param throwOnError boolean
---@param ignoreCase boolean
function System.Type:GetType(typeName, assemblyResolver, typeResolver, throwOnError, ignoreCase) end
---@return string
function System.Type:MakePointerType() end
---@return string
function System.Type:MakeByRefType() end
---@overload fun(): string
---@return string
---@param rank number
function System.Type:MakeArrayType(rank) end
---@overload fun(progID:string): string
---@overload fun(progID:string, throwOnError:boolean): string
---@overload fun(progID:string, server:string): string
---@return string
---@param progID string
---@param server string
---@param throwOnError boolean
function System.Type.GetTypeFromProgID(progID, server, throwOnError) end
---@overload fun(clsid:System.Guid): string
---@overload fun(clsid:System.Guid, throwOnError:boolean): string
---@overload fun(clsid:System.Guid, server:string): string
---@return string
---@param clsid System.Guid
---@param server string
---@param throwOnError boolean
function System.Type.GetTypeFromCLSID(clsid, server, throwOnError) end
---@return number
---@param t string
function System.Type.GetTypeCode(t) end
---@overload fun(name:string, invokeAttr:number, binder:System.Reflection.Binder, target:System.Object, args:System.Object[]): System.Object
---@overload fun(name:string, invokeAttr:number, binder:System.Reflection.Binder, target:System.Object, args:System.Object[], culture:System.Globalization.CultureInfo): System.Object
---@return System.Object
---@param name string
---@param invokeAttr number
---@param binder System.Reflection.Binder
---@param target System.Object
---@param args System.Object[]
---@param modifiers System.Reflection.ParameterModifier[]
---@param culture System.Globalization.CultureInfo
---@param namedParameters System.String[]
function System.Type:InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters) end
---@return System.RuntimeTypeHandle
---@param o System.Object
function System.Type.GetTypeHandle(o) end
---@return number
function System.Type:GetArrayRank() end
---@overload fun(types:System.Type[]): System.Reflection.ConstructorInfo
---@overload fun(bindingAttr:number, binder:System.Reflection.Binder, types:System.Type[], modifiers:System.Reflection.ParameterModifier[]): System.Reflection.ConstructorInfo
---@return System.Reflection.ConstructorInfo
---@param bindingAttr number
---@param binder System.Reflection.Binder
---@param callConvention number
---@param types System.Type[]
---@param modifiers System.Reflection.ParameterModifier[]
function System.Type:GetConstructor(bindingAttr, binder, callConvention, types, modifiers) end
---@overload fun(): System.Reflection.ConstructorInfo[]
---@return System.Reflection.ConstructorInfo[]
---@param bindingAttr number
function System.Type:GetConstructors(bindingAttr) end
---@overload fun(name:string): System.Reflection.MethodInfo
---@overload fun(name:string, types:System.Type[]): System.Reflection.MethodInfo
---@overload fun(name:string, bindingAttr:number): System.Reflection.MethodInfo
---@overload fun(name:string, types:System.Type[], modifiers:System.Reflection.ParameterModifier[]): System.Reflection.MethodInfo
---@overload fun(name:string, bindingAttr:number, binder:System.Reflection.Binder, types:System.Type[], modifiers:System.Reflection.ParameterModifier[]): System.Reflection.MethodInfo
---@return System.Reflection.MethodInfo
---@param name string
---@param bindingAttr number
---@param binder System.Reflection.Binder
---@param callConvention number
---@param types System.Type[]
---@param modifiers System.Reflection.ParameterModifier[]
function System.Type:GetMethod(name, bindingAttr, binder, callConvention, types, modifiers) end
---@overload fun(): System.Reflection.MethodInfo[]
---@return System.Reflection.MethodInfo[]
---@param bindingAttr number
function System.Type:GetMethods(bindingAttr) end
---@overload fun(name:string): System.Reflection.FieldInfo
---@return System.Reflection.FieldInfo
---@param name string
---@param bindingAttr number
function System.Type:GetField(name, bindingAttr) end
---@overload fun(): System.Reflection.FieldInfo[]
---@return System.Reflection.FieldInfo[]
---@param bindingAttr number
function System.Type:GetFields(bindingAttr) end
---@overload fun(name:string): string
---@return string
---@param name string
---@param ignoreCase boolean
function System.Type:GetInterface(name, ignoreCase) end
---@return System.Type[]
function System.Type:GetInterfaces() end
---@return System.Type[]
---@param filter (fun(m:string, filterCriteria:System.Object):boolean)
---@param filterCriteria System.Object
function System.Type:FindInterfaces(filter, filterCriteria) end
---@overload fun(name:string): System.Reflection.EventInfo
---@return System.Reflection.EventInfo
---@param name string
---@param bindingAttr number
function System.Type:GetEvent(name, bindingAttr) end
---@overload fun(): System.Reflection.EventInfo[]
---@return System.Reflection.EventInfo[]
---@param bindingAttr number
function System.Type:GetEvents(bindingAttr) end
---@overload fun(name:string): System.Reflection.PropertyInfo
---@overload fun(name:string, bindingAttr:number): System.Reflection.PropertyInfo
---@overload fun(name:string, types:System.Type[]): System.Reflection.PropertyInfo
---@overload fun(name:string, returnType:string): System.Reflection.PropertyInfo
---@overload fun(name:string, returnType:string, types:System.Type[]): System.Reflection.PropertyInfo
---@overload fun(name:string, returnType:string, types:System.Type[], modifiers:System.Reflection.ParameterModifier[]): System.Reflection.PropertyInfo
---@return System.Reflection.PropertyInfo
---@param name string
---@param bindingAttr number
---@param binder System.Reflection.Binder
---@param returnType string
---@param types System.Type[]
---@param modifiers System.Reflection.ParameterModifier[]
function System.Type:GetProperty(name, bindingAttr, binder, returnType, types, modifiers) end
---@overload fun(): System.Reflection.PropertyInfo[]
---@return System.Reflection.PropertyInfo[]
---@param bindingAttr number
function System.Type:GetProperties(bindingAttr) end
---@overload fun(): System.Type[]
---@return System.Type[]
---@param bindingAttr number
function System.Type:GetNestedTypes(bindingAttr) end
---@overload fun(name:string): string
---@return string
---@param name string
---@param bindingAttr number
function System.Type:GetNestedType(name, bindingAttr) end
---@overload fun(name:string): System.Reflection.MemberInfo[]
---@overload fun(name:string, bindingAttr:number): System.Reflection.MemberInfo[]
---@return System.Reflection.MemberInfo[]
---@param name string
---@param t number
---@param bindingAttr number
function System.Type:GetMember(name, t, bindingAttr) end
---@overload fun(): System.Reflection.MemberInfo[]
---@return System.Reflection.MemberInfo[]
---@param bindingAttr number
function System.Type:GetMembers(bindingAttr) end
---@return System.Reflection.MemberInfo[]
function System.Type:GetDefaultMembers() end
---@return System.Reflection.MemberInfo[]
---@param memberType number
---@param bindingAttr number
---@param filter (fun(m:System.Reflection.MemberInfo, filterCriteria:System.Object):boolean)
---@param filterCriteria System.Object
function System.Type:FindMembers(memberType, bindingAttr, filter, filterCriteria) end
---@return System.Type[]
function System.Type:GetGenericParameterConstraints() end
---@return string
---@param typeArguments System.Type[]
function System.Type:MakeGenericType(typeArguments) end
---@return string
function System.Type:GetElementType() end
---@return System.Type[]
function System.Type:GetGenericArguments() end
---@return string
function System.Type:GetGenericTypeDefinition() end
---@return System.String[]
function System.Type:GetEnumNames() end
---@return System.Array
function System.Type:GetEnumValues() end
---@return string
function System.Type:GetEnumUnderlyingType() end
---@return boolean
---@param value System.Object
function System.Type:IsEnumDefined(value) end
---@return string
---@param value System.Object
function System.Type:GetEnumName(value) end
---@return boolean
---@param c string
function System.Type:IsSubclassOf(c) end
---@return boolean
---@param o System.Object
function System.Type:IsInstanceOfType(o) end
---@return boolean
---@param c string
function System.Type:IsAssignableFrom(c) end
---@return boolean
---@param other string
function System.Type:IsEquivalentTo(other) end
---@return string
function System.Type:ToString() end
---@return System.Type[]
---@param args System.Object[]
function System.Type.GetTypeArray(args) end
---@overload fun(o:System.Object): boolean
---@return boolean
---@param o string
function System.Type:Equals(o) end
---@return boolean
---@param left string
---@param right string
function System.Type.op_Equality(left, right) end
---@return boolean
---@param left string
---@param right string
function System.Type.op_Inequality(left, right) end
---@return number
function System.Type:GetHashCode() end
---@return System.Reflection.InterfaceMapping
---@param interfaceType string
function System.Type:GetInterfaceMap(interfaceType) end
---@return string
---@param typeName string
---@param throwIfNotFound boolean
---@param ignoreCase boolean
function System.Type.ReflectionOnlyGetType(typeName, throwIfNotFound, ignoreCase) end
---@return string
---@param handle System.RuntimeTypeHandle
function System.Type.GetTypeFromHandle(handle) end
---@return boolean
function System.Type:IsConvertableType() end
---@return boolean
function System.Type:CanConvertFromString() end
---@return boolean
function System.Type:CanConvertToString() end
return System.Type
