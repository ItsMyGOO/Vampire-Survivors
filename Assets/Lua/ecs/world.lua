---
--- Created by echo.
--- DateTime: 2025/12/21 21:43
---
---
--- ECS世界管理器，负责实体、组件和系统的管理
--- @class World
--- @field entities table<integer, boolean> 实体ID映射表，entity_id -> true
--- @field components table<string, table<integer, table>> 组件数据存储，component_name -> { entity_id -> component_data }
--- @field systems table 系统列表
local World = {
    entities = {},   -- entity_id -> true
    components = {}, -- component_name -> { entity_id -> component_data }
    systems = {}
}

---
--- 创建并添加一个新的实体
--- @return integer entity_id 实体唯一标识符
function World:AddEntity()
    local eid = require("ecs.entity").Create()
    self.entities[eid] = true
    return eid
end

---
--- 为指定实体添加组件
--- @param eid integer 实体ID
--- @param CompType table 组件类型构造器
--- @param initValues table? 组件初始值表
function World:AddComponent(eid, CompType, initValues)
    local name = CompType.__name
    if not self.components[name] then
        self.components[name] = {}
    end
    local compData = CompType() -- 调用构造器
    for k, v in pairs(initValues or {}) do
        compData[k] = v
    end
    self.components[name][eid] = compData
end

---
--- 获取指定实体的组件数据
--- @param eid integer 实体ID
--- @param CompType table 组件类型构造器
--- @return table|nil 组件数据或nil
function World:GetComponent(eid, CompType)
    local name = CompType.__name
    return self.components[name] and self.components[name][eid]
end

---
--- 检查指定实体是否具有某个组件
--- @param eid integer 实体ID
--- @param CompType table 组件类型构造器
--- @return boolean 是否具有该组件
function World:HasComponent(eid, CompType)
    return self:GetComponent(eid, CompType) ~= nil
end

---
--- 移除指定实体及其所有组件
--- @param eid integer 实体ID
function World:RemoveEntity(eid)
    -- 触发清理事件
    self:emit("entity_removed", eid)

    for name, compTable in pairs(self.components) do
        local comp = compTable[eid]
        if comp and comp.dispose then
            pcall(comp.dispose, comp)
        end
        compTable[eid] = nil
    end
    self.entities[eid] = nil
end

---
--- 更新所有系统
--- @param dt number 帧间隔时间
function World:UpdateSystems(dt)
    for _, sys in ipairs(self.systems) do
        sys:Update(dt)
    end
end

---
--- 添加系统到世界中
--- @param system table 系统实例
function World:AddSystem(system)
    table.insert(self.systems, system)
    system.world = self
end

function World:createView(compTypes)
    local view = {}
    for eid in pairs(self.entities) do
        local match = true
        for _, Comp in ipairs(compTypes) do
            if not self:HasComponent(eid, Comp) then
                match = false; break
            end
        end
        if match then table.insert(view, eid) end
    end
    return view
end

return World
