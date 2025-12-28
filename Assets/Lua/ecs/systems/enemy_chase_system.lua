---
--- Created by echo.
--- DateTime: 2025/12/23 22:28
---
local BaseSystem = require("ecs.base_system")

---@class EnemyAISystem : BaseSystem
local EnemyChaseSystem = {}
EnemyChaseSystem.__index = EnemyChaseSystem

function EnemyChaseSystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, EnemyChaseSystem)
end

---@param world World
function EnemyChaseSystem:start(world)
    BaseSystem.start(self, world)

    local Transform = require("ecs.components.transform")
    local Velocity  = require("ecs.components.velocity")
    local Chase     = require("ecs.components.chase")

    self.transforms = world:GetComponentOfType(Transform)
    self.velocities = world:GetComponentOfType(Velocity)
    self.chase      = world:GetComponentOfType(Chase)
end

function EnemyChaseSystem:update(dt)
    for eid, chase in pairs(self.chase) do
        local vel = self.velocities[eid]
        local trans = self.transforms[eid]
        local targetTrans = self.transforms[chase.target_eid]
        
        if vel and trans and targetTrans then
            local dx = targetTrans.x - trans.x
            local dz = targetTrans.z - trans.z
            local len = math.sqrt(dx * dx + dz * dz)

            if len > 0.001 then
                vel.x = (dx / len) * vel.speed
                vel.z = (dz / len) * vel.speed
            else
                vel.x, vel.z = 0, 0
            end
        end
    end
end

function EnemyChaseSystem:shutdown()
    self.transforms = nil
    self.velocities = nil
    self.chase = nil
end

return EnemyChaseSystem
