---
--- Created by echo.
--- DateTime: 2025/12/23 22:28
---
local BaseSystem = require("ecs.base_system")


--- 敌人追逐系统，负责处理敌人向目标移动的逻辑
---@class EnemyAISystem : BaseSystem
---@field private transforms table<integer, TransformComponent> 存储所有Transform组件的表，key为实体ID，value为TransformComponent
---@field private velocities table<integer, VelocityComponent> 存储所有Velocity组件的表，key为实体ID，value为VelocityComponent
---@field private chase table<integer, ChaseComponent> 存储所有Chase组件的表，key为实体ID，value为ChaseComponent
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

        ---@diagnostic disable-next-line: unnecessary-if
        if vel and trans and targetTrans then
            local dx = targetTrans.x - trans.x
            local dz = targetTrans.z - trans.z
            local len = math.sqrt(dx * dx + dz * dz)

            if len > chase.stop_distance then
                vel.x = (dx / len) * chase.speed
                vel.z = (dz / len) * chase.speed
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
