---
--- Created by echo.
--- DateTime: 2025/12/21 21:46
---
--- 处理实体移动的系统
--- 继承自 BaseSystem，负责根据速度组件更新实体位置
local BaseSystem = require("ecs.base_system")

---@class MovementSystem : BaseSystem
local MovementSystem = {}
MovementSystem.__index = MovementSystem

---@return MovementSystem
function MovementSystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, MovementSystem)
end

---@param world World
function MovementSystem:start(world)
    BaseSystem.start(self, world)

    local Transform = require("ecs.components.transform")
    local Velocity  = require("ecs.components.velocity")

    self.transforms = world:GetComponentOfType(Transform)
    self.velocities = world:GetComponentOfType(Velocity)
end

function MovementSystem:update(dt)
    for eid, vel in pairs(self.velocities) do
        local trans = self.transforms[eid]
        if trans and vel.active ~= false then
            trans.x = trans.x + vel.x * dt
            trans.y = trans.y + vel.y * dt
            trans.z = trans.z + vel.z * dt
            trans.dirty = true
        end
    end
end

function MovementSystem:shutdown()
    self.transforms = nil
    self.velocities = nil
end

return MovementSystem
