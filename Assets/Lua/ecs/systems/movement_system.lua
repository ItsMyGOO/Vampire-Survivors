---
--- Created by echo.
--- DateTime: 2025/12/21 21:46
---
--- 处理实体移动的系统
---@class MovementSystem
local MovementSystem = {}
MovementSystem.__index = MovementSystem

---@param world World
---@param dt number
function MovementSystem:update(world, dt)
    ---@type table<integer, PositionComponent>
    local positions = world:GetComponentOfType(_G.ComponentRegistry.Position)
    ---@type table<integer, VelocityComponent>
    local velocities = world:GetComponentOfType(_G.ComponentRegistry.Velocity)
    ---@type table<integer, SteeringComponent>
    local steerings = world:GetComponentOfType(_G.ComponentRegistry.Steering)

    for eid, steer in pairs(steerings) do
        local vel = velocities[eid]
        vel.x = vel.x + steer.fx * dt;
        vel.y = vel.y + steer.fy * dt;
    end

    for eid, vel in pairs(velocities) do
        local pos = positions[eid]
        if pos and vel.active ~= false then
            pos.x = pos.x + vel.x * dt
            pos.y = pos.y + vel.y * dt
            pos.z = pos.z + vel.z * dt
        end
    end
end

return MovementSystem
