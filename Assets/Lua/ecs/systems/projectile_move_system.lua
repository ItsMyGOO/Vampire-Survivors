---
--- Created by echo.
--- DateTime: 2026/1/7 17:45
---
-- systems/VSProjectileMoveSystem.lua
local VSProjectileMoveSystem = {}
VSProjectileMoveSystem.__index = VSProjectileMoveSystem

function VSProjectileMoveSystem:update(world, dt)
    local C = _G.ComponentRegistry

    local pos = world:GetComponentOfType(C.Position)
    local vel = world:GetComponentOfType(C.Velocity)
    local proj = world:GetComponentOfType(C.Projectile)

    for eid in pairs(proj) do
        local p = pos[eid]
        local v = vel[eid]
        if p and v then
            p.x = p.x + v.vx * dt
            p.y = p.y + v.vy * dt
        end
    end
end

return VSProjectileMoveSystem
