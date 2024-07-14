
LuaHelp = {}

local unpack = table.unpack

-- 解决原生pack的nil截断问题，SafePack与SafeUnpack要成对使用
function LuaHelp.SafePack(...)
	local params = { ... }
	params.n = select("#", ...)
	return params
end

-- 解决原生unpack的nil截断问题，SafePack与SafeUnpack要成对使用
function LuaHelp.SafeUnpack(inSafePackTable)
	return unpack(inSafePackTable, 1, inSafePackTable.n)
end

--- 对两张表进行合并链接
--- @param inSafePackLeft any
--- @param inSafePackRight any
--- @return table
function LuaHelp.ConcatSafePack(inSafePackLeft, inSafePackRight)
	local concat = {}
	for i = 1, inSafePackLeft.n do
		concat[i] = inSafePackLeft[i]
	end
	for i = 1, inSafePackRight.n do
		concat[inSafePackLeft.n + i] = inSafePackRight[i]
	end
	concat.n = inSafePackLeft.n + inSafePackRight.n
	return concat
end

--- 绑定闭包
--- @param self any 对应绑定类
--- @param func function 需要绑定的函数
--- @param ... unknown 参数
--- @return function
function LuaHelp.Bind(self, func, ...)
	assert(self == nil or type(self) == "table")
	assert(func ~= nil and type(func) == "function")
	local params = nil
	if self == nil then
		params = LuaHelp.SafePack(...)
	else
		params = LuaHelp.SafePack(self, ...)
	end
	return function(...)
		local args = LuaHelp.ConcatSafePack(params, LuaHelp.SafePack(...))
		func(LuaHelp.SafeUnpack(args))
	end
end

--- 回调绑定
--- 重载形式：
--- 1、成员函数、私有函数绑定：BindCallback(obj, callback, ...)
--- 2、闭包绑定：BindCallback(callback, ...)
--- @param ... any
--- @return any
function LuaHelp.BindCallback(...)
	local bindFunc = nil
	local params = LuaHelp.SafePack(...)
	assert(params.n >= 1, "BindCallback : error params count!")
	if type(params[1]) == "table" and type(params[2]) == "function" then
		bindFunc = LuaHelp.Bind(...)
	elseif type(params[1]) == "function" then
		bindFunc = LuaHelp.Bind(nil, ...)
	else
		FrostLogE("LuaHelp BindCallback : error params list!")
	end
	return bindFunc
end

--- 是否定义全局变量
--- @param inName string 名字
--- @return any
function LuaHelp.IsDefine(inName)
    return _G[inName]
end

--- 定义全局变量
--- @param inName string 名字
--- @param inValue any 值
--- @return boolean 返回是否注册成功
function LuaHelp.Define(inName, inValue)
	if _G[inName] and inValue then
		FrostLogE("LuaHelp \t register global variable failed, with duplicate name %s", inName)
		return false
	end
	_G[inName] = inValue
    return true
end

----------------------------------------------------------------
--- 查询对应类的父类 是否继承对应类
--- @param inObj any
--- @param inClassName string
--- @return boolean
function LuaHelp.IsChildOfClass(inObj, inClassName)
	local tCurClass = _G[inObj.__classname]
	while tCurClass ~= nil do
		if tCurClass.__classname == inClassName then
			return true
		end
		tCurClass = tCurClass.__super
	end
	
	-- error
	return false
end