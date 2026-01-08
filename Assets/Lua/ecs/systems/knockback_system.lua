---
--- Created by echo.
--- DateTime: 2026/1/8 17:31
---
---@class KnockbackSystem
local KnockbackSystem = {}
KnockbackSystem.__index = KnockbackSystem
local C = _G.ComponentRegistry

function KnockbackSystem:update(world, dt)
    local knockbacks = world:GetComponentOfType(C.Knockback)
    local positions  = world:GetComponentOfType(C.Position)

    for eid, kb in pairs(knockbacks) do
        local pos = positions[eid]
        if pos then
            pos.x = pos.x + kb.forceX * dt
            pos.y = pos.y + kb.forceY * dt
            kb.time = kb.time - dt
            if kb.time <= 0 then
                world:RemoveComponent(eid, C.Knockback)
            end
        end
    end
end

return KnockbackSystem
