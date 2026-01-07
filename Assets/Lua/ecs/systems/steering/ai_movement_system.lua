---
--- Created by echo.
--- DateTime: 2025/12/31 20:04
---
---@class AIMovementSystem
local AIMovementSystem = {}
AIMovementSystem.__index = AIMovementSystem

function AIMovementSystem:update(world, dt)
    local C = _G.ComponentRegistry

    local velocities = world:GetComponentOfType(C.Velocity)
    local intents    = world:GetComponentOfType(C.MoveIntent)
    local steerings  = world:GetComponentOfType(C.Steering)

    for eid, vel in pairs(velocities) do
        local intent   = intents[eid]
        local steering = steerings[eid]
        if not intent or not steering then goto continue end

        -- 1️⃣ 基于 intent 的期望速度（决定“往哪追”）
        local desiredVx = intent.dirX * intent.speed
        local desiredVy = intent.dirY * intent.speed

        -- 2️⃣ 平滑逼近（一阶响应）
        local accel = 8
        local t = 1 - math.exp(-accel * dt)
        vel.x = vel.x + (desiredVx - vel.x) * t
        vel.y = vel.y + (desiredVy - vel.y) * t

        -- 3️⃣ separation：只扰动速度方向，不改变朝向
        local vx, vy = vel.x, vel.y
        local speedSq = vx * vx + vy * vy

        if speedSq > 1e-6 then
            local speed = math.sqrt(speedSq)
            local nx, ny = vx / speed, vy / speed

            -- ⭐ 横向扰动（不乘 dt，表现为“位置修正”）
            nx = nx + steering.sepFx
            ny = ny + steering.sepFy

            local nlen = math.sqrt(nx * nx + ny * ny)
            if nlen > 1e-6 then
                nx = nx / nlen
                ny = ny / nlen
                vel.x = nx * speed
                vel.y = ny * speed
            end
        end

        ::continue::
    end
end

return AIMovementSystem
