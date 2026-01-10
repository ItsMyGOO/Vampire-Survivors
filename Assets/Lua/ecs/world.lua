---
--- Created by echo.
--- DateTime: 2025/12/21 21:43
---
---
--- 核心实体-组件存储系统
--- 设计原则：只做容器管理，不含任何游戏逻辑
--- @class World
--- @field time number 当前时间
--- @field private components table 组件池：componentName -> eid -> component_data
--- @field nextEntityId integer 下一个实体ID
--- @field eventBus EventBus? 事件总线
--- @field player_eid integer 玩家实体ID
--- @field grid Grid? 逻辑网格
--- @field player Player? 玩家实例
local World = {
    time = 0.0,
    nextEntityId = 1,
    components = {},
    eventBus = nil,
    player_eid = -1,
    grid = nil,
    player = nil
}
World.__index = World

---
--- 创建 World 实例
--- @return World
function World.New()
    local self = {
        nextEntityId = 1,
        components = {},
        eventBus = nil,
        player = -1
    }
    return setmetatable(self, World)
end

---
--- 创建新实体
--- @return integer entity_id 实体唯一标识符
function World:CreateEntity()
    local eid = self.nextEntityId
    self.nextEntityId = eid + 1
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

---
--- 获取组件
--- @param eid integer 实体ID
--- @param schema table 组件模式表
--- @return table? 若不存在返回 nil
function World:GetComponent(eid, schema)
    local pool = self.components[schema]
    return pool and pool[eid]
end

--- 获取所有指定类型的组件
--- @param schema table 组件模式表
--- @return table<integer, table> eid -> component_data 映射表，键为实体ID，值为组件数据表
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
    -- 遍历所有组件池，清理该 eid
    for _, pool in pairs(self.components) do
        if pool[eid] then
            pool[eid] = nil
        end
    end

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
