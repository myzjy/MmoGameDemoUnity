
local LuaHelp = {}

-- 解决原生pack的nil截断问题，SafePack与SafeUnpack要成对使用
function LuaHelp.SafePack(...)
	local params = { ... }
	params.n = select("#", ...)
	return params
end

function LuaHelp.ConcatSafePack(safe_pack_l, safe_pack_r)
	local concat = {}
	for i = 1, safe_pack_l.n do
		concat[i] = safe_pack_l[i]
	end
	for i = 1, safe_pack_r.n do
		concat[safe_pack_l.n + i] = safe_pack_r[i]
	end
	concat.n = safe_pack_l.n + safe_pack_r.n
	return concat
end


return LuaHelp