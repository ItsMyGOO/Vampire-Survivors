---
--- Created by echo.
--- DateTime: 2026/1/7 17:44
---
-- weapon/WeaponSteps.lua
local Steps = {}

-- ---------- 基础计算 ----------

Steps.CalcInterval = function(ctx)
    ctx.interval = ctx.weapon.interval
end

Steps.CalcCount = function(ctx)
    ctx.count = ctx.weapon.base_count or 1
end

-- ---------- 目标选择 ----------

Steps.SelectTarget = function(ctx)
    local world = ctx.world
    local C = _G.ComponentRegistry

    local ownerPos = world:GetComponent(ctx.owner, C.Position)
    if not ownerPos then return end

    local enemies = world:GetComponentOfType(C.EnemyTag)
    local positions = world:GetComponentOfType(C.Position)

    local best, bestD2 = nil, math.huge
    for eid in pairs(enemies) do
        local pos = positions[eid]
        if pos then
            local dx = pos.x - ownerPos.x
            local dy = pos.y - ownerPos.y
            local d2 = dx*dx + dy*dy
            if d2 < bestD2 then
                bestD2 = d2
                best = eid
            end
        end
    end

    ctx.target = best
end

-- ---------- 朝目标方向 ----------

Steps.CalcDirection = function(ctx)
    local world = ctx.world
    local C = _G.ComponentRegistry

    local ownerPos = world:GetComponent(ctx.owner, C.Position)
    local targetPos = ctx.target and world:GetComponent(ctx.target, C.Position)

    if not ownerPos or not targetPos then
        ctx.dirX, ctx.dirY = 1, 0
        return
    end

    local dx = targetPos.x - ownerPos.x
    local dy = targetPos.y - ownerPos.y
    local len = math.sqrt(dx*dx + dy*dy)

    if len > 1e-6 then
        ctx.dirX, ctx.dirY = dx/len, dy/len
    else
        ctx.dirX, ctx.dirY = 1, 0
    end
end

-- ---------- 发射投射物 ----------

Steps.EmitProjectile = function(ctx)
    local C = _G.ComponentRegistry
    local world = ctx.world
    local weapon = ctx.weapon

    local ownerPos = world:GetComponent(ctx.owner, C.Position)
    if not ownerPos then return end

    for i = 1, ctx.count do
        local eid = world:CreateEntity()

        world:AddComponent(eid, C.Position, {
            x = ownerPos.x,
            y = ownerPos.y
        })

        world:AddComponent(eid, C.Velocity, {
            vx = ctx.dirX * weapon.base_speed,
            vy = ctx.dirY * weapon.base_speed
        })

        world:AddComponent(eid, C.Projectile, {
            damage = weapon.base_damage,
            owner = ctx.owner,
            lifetime = 1.5
        })
    end
end

-- ---------- 环绕型武器 ----------

Steps.EmitOrbit = function(ctx)
    local C = _G.ComponentRegistry
    local world = ctx.world
    local weapon = ctx.weapon

    local eid = world:CreateEntity()

    world:AddComponent(eid, C.Position, { x = 0, y = 0 })

    world:AddComponent(eid, C.Orbit, {
        owner = ctx.owner,
        radius = weapon.radius,
        angle = math.random() * math.pi * 2,
        angularSpeed = weapon.angularSpeed
    })

    world:AddComponent(eid, C.Projectile, {
        damage = weapon.base_damage,
        owner = ctx.owner,
        lifetime = math.huge
    })
end

return Steps
