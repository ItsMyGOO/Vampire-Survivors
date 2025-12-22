---
--- Created by echo.
--- DateTime: 2025/12/21 21:43
---
-- ecs/world.lua
-- 核心实体-组件存储系统
-- 设计原则：只做容器管理，不含任何游戏逻辑

local World = {
    ---@private 实体集合
    entities = {}, -- eid -> true

    ---@private 组件池：componentName -> eid -> component_data
    components = {}, -- string -> table<integer, table>

    ---@type EventBus?
    eventBus = nil,

    ---@private 组件名缓存：schema -> name（避免重复查找）
    schemaNames = {}
}

--- 创建新实体
function World:AddEntity()
    local eid = require("ecs.entity").Create()
    self.entities[eid] = true
    return eid
end

--- 添加组件到实体
-- @param eid integer
-- @param schema table 来自 require("components.xxx")
-- @param override? table 覆盖字段
function World:AddComponent(eid, schema, override)
    local name = self:getSchemaName(schema)

    -- 初始化组件池
    if not self.components[name] then
        self.components[name] = {}
    end

    -- 深拷贝默认值 + 合并覆盖
    local comp = self:deepCopy(schema)
    if override then
        for k, v in pairs(override) do
            comp[k] = v
        end
    end

    self.components[name][eid] = comp

    -- 可选：触发事件
    if self.eventBus then
        self.eventBus:emit("component_added", {
            entityId = eid,
            componentName = name,
            component = comp
        })
    end
end

--- 获取组件
-- @return table? 若不存在返回 nil
function World:GetComponent(eid, schema)
    local name = self:getSchemaName(schema)
    local pool = self.components[name]
    return pool and pool[eid]
end

--- 判断是否拥有某组件
function World:HasComponent(eid, schema)
    return self:GetComponent(eid, schema) ~= nil
end

--- 移除组件
function World:RemoveComponent(eid, schema)
    local name = self:getSchemaName(schema)
    local pool = self.components[name]
    if pool and pool[eid] then
        pool[eid] = nil

        if self.eventBus then
            self.eventBus:emit("component_removed", {
                entityId = eid,
                componentName = name
            })
        end
    end
end

--- 销毁实体（移除所有组件）
function World:DestroyEntity(eid)
    if not self.entities[eid] then return end

    -- 遍历所有组件池，清理该 eid
    for name, pool in pairs(self.components) do
        if pool[eid] then
            pool[eid] = nil
        end
    end

    self.entities[eid] = nil

    if self.eventBus then
        self.eventBus:emit("entity_destroyed", { entityId = eid })
    end
end

-- ---------------------------------------
-- 内部工具方法
-- ---------------------------------------

--- 获取组件名称（缓存 + 回退）
-- @param schema table
-- @return string 如 "Transform"
function World:getSchemaName(schema)
    -- 优先从缓存读
    local cached = self.schemaNames[schema]
    if cached then return cached end

    -- 从元表获取
    local mt = getmetatable(schema)
    if mt and mt.__name then
        self.schemaNames[schema] = mt.__name
        return mt.__name
    end

    -- 回退：遍历 package.loaded（仅一次）
    for fullName, mod in pairs(package.loaded) do
        if mod == schema then
            local shortName = fullName:match("^.-(%w+)$") or "unknown"
            self.schemaNames[schema] = shortName
            return shortName
        end
    end

    error("Failed to determine schema name for component table", 2)
end

--- 深拷贝（用于创建组件实例）
-- 注意：不处理函数和 userdata
function World:deepCopy(orig)
    local copy = {}
    for k, v in pairs(orig) do
        if type(v) == "table" then
            copy[k] = self:deepCopy(v)
        else
            copy[k] = v
        end
    end
    return copy
end

return World
