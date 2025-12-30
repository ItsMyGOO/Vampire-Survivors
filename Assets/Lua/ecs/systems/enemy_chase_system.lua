---
--- Created by echo.
--- DateTime: 2025/12/23 22:28
---
--- 敌人追逐系统，负责处理敌人向目标移动的逻辑
---@class EnemyAISystem
---@field private positions table<integer, PositionComponent> 存储所有Transform组件的表，key为实体ID，value为TransformComponent
---@field private velocities table<integer, VelocityComponent> 存储所有Velocity组件的表，key为实体ID，value为VelocityComponent
---@field private chase table<integer, ChaseComponent> 存储所有Chase组件的表，key为实体ID，value为ChaseComponent
local EnemyChaseSystem = {}
EnemyChaseSystem.__index = EnemyChaseSystem

---@param world World
---@param dt number
function EnemyChaseSystem:update(world, dt)
    ---@type table<integer, PositionComponent>
    local positions = world:GetComponentOfType(_G.ComponentRegistry.Position)
    ---@type table<integer, VelocityComponent>
    local velocities = world:GetComponentOfType(_G.ComponentRegistry.Velocity)
    ---@type table<integer, ChaseComponent>
    local chases = world:GetComponentOfType(_G.ComponentRegistry.Chase)

    for eid, chase in pairs(chases) do
        local vel = velocities[eid]
        local trans = positions[eid]
        local targetTrans = positions[chase.target_eid]

        local dx = targetTrans.x - trans.x
        local dy = targetTrans.y - trans.y
        local dz = targetTrans.z - trans.z
        local len = math.sqrt(dx * dx + dy * dy + dz * dz)

        if len > chase.stop_distance then
            vel.x = (dx / len) * chase.speed
            vel.y = (dy / len) * chase.speed
            vel.z = (dz / len) * chase.speed
        else
            vel.x, vel.y, vel.z = 0, 0, 0
        end
    end
end

return EnemyChaseSystem
