---
--- Created by echo.
--- DateTime: 2025/12/21 21:43
---
local World = {
    entities = {},          -- entity_id -> true
    components = {},        -- component_name -> { entity_id -> component_data }
    systems = {}
}

function World:AddEntity()
    local eid = require("ecs.entity").Create()
    self.entities[eid] = true
    return eid
end

function World:AddComponent(eid, CompType, initValues)
    local name = CompType.__name
    if not self.components[name] then
        self.components[name] = {}
    end
    local compData = CompType()  -- 调用构造器
    for k, v in pairs(initValues or {}) do
        compData[k] = v
    end
    self.components[name][eid] = compData
end

function World:GetComponent(eid, CompType)
    local name = CompType.__name
    return self.components[name] and self.components[name][eid]
end

function World:HasComponent(eid, CompType)
    return self:GetComponent(eid, CompType) ~= nil
end

function World:RemoveEntity(eid)
    for _, compTable in pairs(self.components) do
        compTable[eid] = nil
    end
    self.entities[eid] = nil
end

-- 执行所有系统
function World:UpdateSystems(dt)
    for _, sys in ipairs(self.systems) do
        sys:Update(dt)
    end
end

-- 注册系统
function World:AddSystem(system)
    table.insert(self.systems, system)
    system.world = self
end

return World