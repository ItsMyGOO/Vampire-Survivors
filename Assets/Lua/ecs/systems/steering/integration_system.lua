---
--- Created by echo.
--- DateTime: 2025/12/21 21:46
---
--- 处理实体移动的系统
---@class IntegrationSystem
local IntegrationSystem = {}
IntegrationSystem.__index = IntegrationSystem

---@param world World
---@param dt number
function IntegrationSystem:update(world, dt)
    local C = _G.ComponentRegistry
    local positions  = world:GetComponentOfType(C.Position)
    local velocities = world:GetComponentOfType(C.Velocity)

    for eid, vel in pairs(velocities) do
        local pos = positions[eid]
        if pos and vel.active ~= false then
            pos.x = pos.x + vel.x * dt
            pos.y = pos.y + vel.y * dt
            pos.z = pos.z + vel.z * dt
        end
    end
end

return IntegrationSystem
