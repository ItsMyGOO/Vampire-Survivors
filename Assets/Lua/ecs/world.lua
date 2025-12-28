---
--- Created by echo.
--- DateTime: 2025/12/21 21:43
---
---
--- 核心实体-组件存储系统
--- 设计原则：只做容器管理，不含任何游戏逻辑
--- @class World
--- @field private entities table 实体集合 eid -> true
--- @field private components table 组件池：componentName -> eid -> component_data
--- @field private systems table 系统集合：systemName -> system_data
--- @field eventBus EventBus? 事件总线
--- @field private schemaNames table 组件名缓存：schema -> name（避免重复查找）
--- @field player_eid integer 玩家实体ID
local World = {
    ---@private
    entities = {}, -- eid -> true

    ---@private
    components = {}, -- string -> table<integer, table>

    ---@private
    systems = {}, -- string -> table

    ---@type EventBus?
    eventBus = nil,

    ---@private
    schemaNames = {},

    player_eid = -1,
}

---
--- 创建新实体
--- @return integer entity_id 实体唯一标识符
function World:AddEntity()
    local eid = require("ecs.entity").Create()
    self.entities[eid] = true
    return eid
end

---
--- 添加组件到实体
--- @param eid integer 实体ID
--- @param schema table 来自 require("components.xxx")
--- @param override? table 覆盖字段
function World:AddComponent(eid, schema, override)
    -- 初始化组件池
    if not self.components[schema] then
        self.components[schema] = {}
    end

    -- 深拷贝默认值 + 合并覆盖
    local comp = self:deepCopy(schema)
    if override then
        for k, v in pairs(override) do
            comp[k] = v
        end
    end

    self.components[schema][eid] = comp

    -- 可选：触发事件
    if self.eventBus then
        self.eventBus:emit("component_added", {
            entityId = eid,
            componentName = schema,
            component = comp
        })
    end
end

--- 添加一个系统到世界
--- @param system table 必须有 start(world) 和 update(dt)
function World:AddSystem(system)
    table.insert(self.systems, system)

    -- 如果系统有 start 方法，立即调用
    if system.start then
        system:start(self)
    end
end

--- 更新所有系统
--- @param dt number 增量时间（秒）
function World:UpdateSystems(dt)
    for _, system in ipairs(self.systems) do
        if system.update then
            -- 使用 xpcall 防止单个系统崩溃导致整个游戏退出
            local ok, err = xpcall(system.update, debug.traceback, system, dt)
            if not ok then
                print("❌ Lua Error in system.update:")
                print(err)
            end
        end
    end
end

---
--- 获取组件
--- @param eid integer 实体ID
--- @param schema table 组件模式表
--- @return table? 若不存在返回 nil
function World:GetComponent(eid, schema)
    local pool = self.components[schema]
    return pool and pool[eid]
end

---
--- 获取所有schema类型组件
--- @param schema table 组件模式表
--- @return table components[schema]
function World:GetComponentOfType(schema)
    local components = self.components[schema]
    if not components then
        components = {}
        self.components[schema] = components
    end
    return components
end

---
--- 判断是否拥有某组件
--- @param eid integer 实体ID
--- @param schema table 组件模式表
--- @return boolean 是否拥有该组件
function World:HasComponent(eid, schema)
    return self:GetComponent(eid, schema) ~= nil
end

---
--- 移除组件
--- @param eid integer 实体ID
--- @param schema table 组件模式表
function World:RemoveComponent(eid, schema)
    local pool = self.components[schema]
    if pool and pool[eid] then
        pool[eid] = nil

        if self.eventBus then
            self.eventBus:emit("component_removed", {
                entityId = eid,
                componentName = schema
            })
        end
    end
end

---
--- 销毁实体（移除所有组件）
--- @param eid integer 实体ID
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
---
--- 深拷贝（用于创建组件实例）
--- 注意：不处理函数和 userdata
--- @param orig table 原始表
--- @return table 拷贝后的表
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
