---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/11 19:48
---
function PrintTable(tbl, level, return_counter)
	if tbl == nil or type(tbl) ~= "table" then
		return
	end
	return_counter = return_counter or 7 --剩下多少层就返回,防止无限打印
	if return_counter <= 0 then
		-- print('Cat:util.lua PrintTable return_counter empty')
		return
	end
	return_counter = return_counter - 1
	level = level or 1

	local indent_str = ""
	for i = 1, level do
		indent_str = indent_str .. "	"
	end
	print(indent_str .. "{")
	for k, v in pairs(tbl) do
		local item_str = string.format("%s%s = %s", indent_str .. "	", tostring(k), tostring(v))
		print(item_str)
		if type(v) == "table" then
			PrintTable(v, level + 1, return_counter)
		end
	end
	print(indent_str .. "}")
end

--将 szFullString 对象拆分为一个子字符串表
function Split(szFullString, szSeparator, start_pos)
	local nFindStartIndex = start_pos or 1
	local nSplitIndex = 1
	local nSplitArray = {}
	while true do
		local nFindLastIndex = string.find(szFullString, szSeparator, nFindStartIndex)
		if not nFindLastIndex then
			nSplitArray[nSplitIndex] = string.sub(szFullString, nFindStartIndex, string.len(szFullString))
			break
		end
		table.insert(nSplitArray, string.sub(szFullString, nFindStartIndex, nFindLastIndex - 1))
		nFindStartIndex = nFindLastIndex + string.len(szSeparator)
		nSplitIndex = nSplitIndex + 1
	end
	return nSplitArray
end

function Trim(str)
	if str == nil or type(str) == "table" then
		return ""
	end
	str = string.gsub(str, "^[ \t\n\r]+", "")
	return string.gsub(str, "[ \t\n\r]+$", "")
end

function Round(number)
	local intNum = math.floor(number)
	if number >= (intNum + 0.5) then
		return intNum + 1
	else
		return intNum
	end
end
local move_end = {}

local unpack = unpack or table.unpack

local generator_mt = {
	__index = {
		MoveNext = function(self)
			self.Current = self.co()
			if self.Current == move_end then
				self.Current = nil
				return false
			else
				return true
			end
		end,
		Reset = function(self)
			self.co = coroutine.wrap(self.w_func)
		end,
	},
}

function cs_generator(func, ...)
	local params = { ... }
	local generator = setmetatable({
		w_func = function()
			func(unpack(params))
			return move_end
		end,
	}, generator_mt)
	generator:Reset()
	return generator
end
function coroutine_call(func)
	return function(...)
		local co = coroutine.create(func)
		assert(coroutine.resume(co, ...))
	end
end

function hotfix_ex(cs, field, func)
	assert(
		type(field) == "string" and type(func) == "function",
		"invalid argument: #2 string needed, #3 function needed!"
	)
	local function func_after(...)
		xlua.hotfix(cs, field, nil)
		local ret = { func(...) }
		xlua.hotfix(cs, field, func_after)
		return unpack(ret)
	end
	xlua.hotfix(cs, field, func_after)
end
