---
--- Created by echo.
--- DateTime: 2026/1/7 17:45
---
-- systems/VSOrbitSystem.lua
local VSOrbitSystem = {}
VSOrbitSystem.__index = VSOrbitSystem

function VSOrbitSystem:update(world, dt)
    local C = _G.ComponentRegistry

    local orbits = world:GetComponentOfType(C.Orbit)
    local pos = world:GetComponentOfType(C.Position)

    for eid, orbit in pairs(orbits) do
        local ownerPos = pos[orbit.owner]
        local p = pos[eid]
        if ownerPos and p then
            orbit.angle = orbit.angle + orbit.angularSpeed * dt
            p.x = ownerPos.x + math.cos(orbit.angle) * orbit.radius
            p.y = ownerPos.y + math.sin(orbit.angle) * orbit.radius
        end
    end
end

return VSOrbitSystem
