---
--- Created by echo.
--- DateTime: 2026/1/7 16:58
---
-- weapon/FirePatterns.lua
local FirePatterns = {}

function FirePatterns.Forward(world, owner, pos, def)
    local C = _G.ComponentRegistry

    for i = 1, def.count do
        local eid = world:CreateEntity()

        world:AddComponent(eid, C.Position, {
            x = pos.x,
            y = pos.y
        })

        world:AddComponent(eid, C.Velocity, {
            vx = 1,
            vy = 0
        })

        world:AddComponent(eid, C.Projectile, {
            damage = 1,
            owner = owner
        })
    end
end

function FirePatterns.Arc(world, owner, pos, def)
    local C = _G.ComponentRegistry
    local angleStep = math.pi / 6

    for i = 1, def.count do
        local angle = -angleStep + (i - 1) * angleStep

        local eid = world:CreateEntity()
        world:AddComponent(eid, C.Position, { x = pos.x, y = pos.y })
        world:AddComponent(eid, C.Velocity, {
            vx = math.cos(angle),
            vy = math.sin(angle)
        })
        world:AddComponent(eid, C.Projectile, {
            damage = 2,
            owner = owner
        })
    end
end

return FirePatterns
