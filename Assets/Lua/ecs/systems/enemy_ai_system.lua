---
--- Created by echo.
--- DateTime: 2025/12/23 22:28
---
local BaseSystem = require("ecs.base_system")

---@class EnemyAISystem : BaseSystem
local EnemyAISystem = {}
EnemyAISystem.__index = EnemyAISystem

function EnemyAISystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, EnemyAISystem)
end

function EnemyAISystem:start(world)
    BaseSystem.start(self, world)

    local EnemyTag  = require("ecs.components.enemy_tag")
    local Transform = require("ecs.components.transform")
    local Velocity  = require("ecs.components.velocity")
    local Target    = require("ecs.components.target")

    self.enemies    = world:GetComponentOfType(EnemyTag)
    self.transforms = world:GetComponentOfType(Transform)
    self.velocities = world:GetComponentOfType(Velocity)
    self.targets    = world:GetComponentOfType(Target)
end

function EnemyAISystem:update(dt)
    for eid in pairs(self.enemies) do
        local target = self.targets[eid]
        local vel = self.velocities[eid]
        local trans = self.transforms[eid]

        if target and vel and trans then
            local targetTrans = self.transforms[target.eid]
            if targetTrans then
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
end

function EnemyAISystem:shutdown()
    self.enemies = nil
    self.transforms = nil
    self.velocities = nil
    self.targets = nil
end

return EnemyAISystem
