---
--- Created by echo.
--- DateTime: 2025/12/31 20:04
---
--- @class AIMovementSystem
local AIMovementSystem = {}
AIMovementSystem.__index = AIMovementSystem

function AIMovementSystem:update(world, dt)
    local C          = _G.ComponentRegistry

    local velocities = world:GetComponentOfType(C.Velocity)
    local intents    = world:GetComponentOfType(C.MoveIntent)
    local steerings  = world:GetComponentOfType(C.Steering)

    for eid, steering in pairs(steerings) do
        local vel = velocities[eid]
        local intent = intents[eid]
        if not vel or not intent then goto continue end

        -- 1️⃣ 基于 intent 的期望速度（seek 决定）
        local desiredVx = intent.dirX * intent.speed
        local desiredVy = intent.dirY * intent.speed

        -- 2️⃣ 平滑逼近期望速度（解决“冲过头”）
        local accel = 8
        local t = 1 - math.exp(-accel * dt)
        vel.x = vel.x + (desiredVx - vel.x) * t
        vel.y = vel.y + (desiredVy - vel.y) * t

        -- 3️⃣ 分离只扰动速度（不影响朝向）
        vel.x = vel.x + steering.sepFx * dt
        vel.y = vel.y + steering.sepFy * dt

        -- 4️⃣ 限速
        --local len = math.sqrt(vel.x * vel.x + vel.y * vel.y)
        --if len > vel.speed then
        --    vel.x = vel.x / len * vel.speed
        --    vel.y = vel.y / len * vel.speed
        --end

        ::continue::
    end
end

return AIMovementSystem
