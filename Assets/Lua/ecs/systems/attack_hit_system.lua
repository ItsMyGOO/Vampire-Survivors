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

        -- 命中缓存（防御式）
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

            if not inRange then
                -- persistent：离开范围时清理 timer
                if src.mode == "persistent" then
                    src.hitTargets[eid] = nil
                end
                goto continue_enemy
            end

            if src.mode == "persistent" then
                -- 法球 / 光环 / 激光：按 tick 伤害
                local t = (src.hitTargets[eid] or 0) + dt
                if t < src.tickInterval then
                    src.hitTargets[eid] = t
                    goto continue_enemy
                end

                -- ★ 到这里，说明要结算一次伤害
                src.hitTargets[eid] = t - src.tickInterval
            else
                -- single / pierce：每个敌人只命中一次
                if src.hitTargets[eid] then
                    goto continue_enemy
                end
                src.hitTargets[eid] = true
            end
            -- 伤害
            hp.value = hp.value - src.damage

            -- 击退
            local kx, ky
            if src.knockbackMode == "fromOwner" then
                local ownerPos = positions[src.owner]
                kx = epos.x - ownerPos.x
                ky = epos.y - ownerPos.y
            else
                kx = epos.x - spos.x
                ky = epos.y - spos.y
            end

            local len = math.sqrt(kx * kx + ky * ky)
            if len > 1e-6 then
                world:AddComponent(eid, C.Knockback, {
                    forceX = kx / len * src.knockback,
                    forceY = ky / len * src.knockback,
                    time   = 0.15
                })
            end

            if src.mode == "single" then
                world:DestroyEntity(sid)
                -- 销毁，不再检测下一个敌人
                goto continue_src
            end

            ::continue_enemy::
        end

        ::continue_src::
    end
end

return AttackHitSystem
