---
--- Created by echo.
--- DateTime: 2026/1/9 14:55
---
local PropConfig = require("Data.prop_config")

EnemyDeathSystem = {}

function EnemyDeathSystem:update(world, dt)
    local C       = _G.ComponentRegistry

    local enemies = world:GetComponentOfType(C.EnemyTag)
    local healths = world:GetComponentOfType(C.Health)
    local pos     = world:GetComponentOfType(C.Position)

    for eid, _ in pairs(enemies) do
        local hp = healths[eid]
        if hp and hp.value <= 0 then
            local p = pos[eid]

            -- 掉落经验
            if p then
                local cfg = PropConfig.exp_small

                local orb = world:CreateEntity()
                world:AddComponent(orb, C.Position, {
                    x = p.x,
                    y = p.y,
                    z = 0
                })
                world:AddComponent(orb, C.Collider, { radius = 0.4 })
                world:AddComponent(orb, C.Prop, { exp = cfg.exp })
                world:AddComponent(orb, C.SpriteKey, {
                    sheet = PropConfig.sheet,
                    key = cfg.key
                })
            end

            world:DestroyEntity(eid)
        end
    end
end

return EnemyDeathSystem
