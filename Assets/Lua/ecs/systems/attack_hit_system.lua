---
--- Created by echo.
--- DateTime: 2026/1/8 17:10
---
---@class AttackHitSystem
local AttackHitSystem = {}
AttackHitSystem.__index = AttackHitSystem
local C = _G.ComponentRegistry

function AttackHitSystem:update(world, dt)
    local sources   = world:GetComponentOfType(C.DamageSource)
    local colliders = world:GetComponentOfType(C.Collider)
    local positions = world:GetComponentOfType(C.Position)

    local enemies   = world:GetComponentOfType(C.EnemyTag)
    local healths   = world:GetComponentOfType(C.Health)

    for sid, src in pairs(sources) do
        local spos = positions[sid]
        local scol = colliders[sid]
        if not (spos and scol) then goto continue_src end

        for eid, _ in pairs(enemies) do
            local epos = positions[eid]
            local ecol = colliders[eid]
            local hp   = healths[eid]
            if not (epos and ecol and hp) then goto continue_enemy end

            local dx = epos.x - spos.x
            local dy = epos.y - spos.y
            local r  = scol.radius + ecol.radius

            if dx * dx + dy * dy <= r * r then
                -- 伤害
                hp.value = hp.value - src.damage

                -- 击退
                if src.knockback and src.knockback > 0 then
                    local len = math.sqrt(dx * dx + dy * dy)
                    if len > 1e-6 then
                        world:AddComponent(eid, C.Knockback, {
                            forceX = dx / len * src.knockback,
                            forceY = dy / len * src.knockback,
                            time   = 0.15
                        })
                    end
                end

                -- 是否只命中一次
                if src.hitOnce then
                    world:DestroyEntity(sid)
                    goto continue_src
                end
            end

            ::continue_enemy::
        end

        ::continue_src::
    end
end

return AttackHitSystem
