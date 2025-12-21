---
--- Created by echo.
--- DateTime: 2025/12/21 21:39
---
local Component = {}
local components = {} -- name -> component class

function deepcopy(obj)
    if type(obj) ~= "table" then return obj end
    local res = {}
    for k, v in pairs(obj) do
        res[deepcopy(k)] = deepcopy(v)
    end
    return setmetatable(res, getmetatable(obj))
end

function Component.Register(name, defaultValues)
    local comp = {
        __name = name,
        __default = defaultValues or {},
    }
    setmetatable(comp, {
        __call = function()
            return deepcopy(comp.__default) -- 返回默认值副本
        end
    })
    components[name] = comp
    return comp
end

function Component.Get(name)
    return components[name]
end

return Component
