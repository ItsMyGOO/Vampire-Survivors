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

        -- 命中缓存
        src.hitTargets = src.hitTargets or {}

        for eid, _ in pairs(enemies) do
            local epos = positions[eid]
            local ecol = colliders[eid]
            local hp   = healths[eid]
            if not (epos and ecol and hp) then
                goto continue_enemy
            end

            -- 圆形碰撞检测
            local dx      = epos.x - spos.x
            local dy      = epos.y - spos.y
            local r       = scol.radius + ecol.radius
            local inRange = (dx * dx + dy * dy) <= r * r

            -- 不在范围
            if not inRange then
                -- persistent：离开范围就清 tick
                if src.mode == "persistent" then
                    src.hitTargets[eid] = nil
                end
                goto continue_enemy
            end

            -- ===============================
            -- 命中判定
            -- ===============================

            if src.mode == "persistent" then
                local t = src.hitTargets[eid]

                if not t then
                    -- 第一次进入范围：立刻结算一次伤害
                    src.hitTargets[eid] = 0
                else
                    t = t + dt

                    if t < src.tickInterval then
                        src.hitTargets[eid] = t
                        goto continue_enemy
                    end

                    src.hitTargets[eid] = t - src.tickInterval
                end
            else
                -- single / pierce：只命中一次
                if src.hitTargets[eid] then
                    goto continue_enemy
                end
                src.hitTargets[eid] = true
            end

            -- ===============================
            -- 结算伤害
            -- ===============================
            hp.value = hp.value - src.damage
            world:AddComponent(eid, C.AnimationCommand, {
                play = "Hit",
                forceRestart = true
            })

            -- ===============================
            -- 击退
            -- ===============================
            local kx, ky
            if src.knockbackMode == "fromOwner" then
                local ownerPos = positions[src.owner]
                if ownerPos then
                    kx = epos.x - ownerPos.x
                    ky = epos.y - ownerPos.y
                else
                    kx = dx
                    ky = dy
                end
            else
                kx = dx
                ky = dy
            end

            local len = math.sqrt(kx * kx + ky * ky)
            if len > 1e-6 then
                world:AddComponent(eid, C.Knockback, {
                    forceX = kx / len * src.knockback,
                    forceY = ky / len * src.knockback,
                    time   = 0.15
                })
            end

            -- single 命中即销毁
            if src.mode == "single" then
                world:DestroyEntity(sid)
                goto continue_src
            end

            ::continue_enemy::
        end

        ::continue_src::
    end
end

return AttackHitSystem
