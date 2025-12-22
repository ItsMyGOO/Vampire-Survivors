---
--- Created by echo.
--- DateTime: 2025/12/21 21:39
---
--- 组件注册和管理系统
--- @class Component
local Component = {}
local components = {} -- name -> component class

---
--- 深拷贝函数，用于复制表结构
--- @param obj any 需要深拷贝的对象
--- @return any 深拷贝后的对象
function deepcopy(obj)
    if type(obj) ~= "table" then return obj end
    local res = {}
    for k, v in pairs(obj) do
        res[deepcopy(k)] = deepcopy(v)
    end
    return setmetatable(res, getmetatable(obj))
end

---
--- 注册一个新的组件类型
--- @param name string 组件名称
--- @param defaultValues table? 组件默认值表
--- @return table 注册的组件类
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

---
--- 获取已注册的组件类型
--- @param name string 组件名称
--- @return table|nil 组件类或nil
function Component.Get(name)
    return components[name]
end

return Component
