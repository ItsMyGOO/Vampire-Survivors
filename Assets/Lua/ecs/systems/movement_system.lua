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
