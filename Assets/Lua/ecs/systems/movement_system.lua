---
--- Created by echo.
--- DateTime: 2026/1/1 20:40
---
---@class MovementSystem
local MovementSystem = {}
MovementSystem.__index = MovementSystem

function MovementSystem:update(world, dt)
    local C = _G.ComponentRegistry

    local positions = world:GetComponentOfType(C.Position)
    local intents   = world:GetComponentOfType(C.MoveIntent)

    for eid, intent in pairs(intents) do
        local pos = positions[eid]
        if not pos then goto continue end

        local speed = intent.speed or 0
        local dx    = intent.dirX or 0
        local dy    = intent.dirY or 0

        -- 归一化
        local lenSq = dx * dx + dy * dy
        if lenSq > 1e-6 then
            local invLen = 1 / math.sqrt(lenSq)
            dx = dx * invLen
            dy = dy * invLen
        else
            dx, dy = 0, 0
        end

        pos.x = pos.x + dx * speed * dt
        pos.y = pos.y + dy * speed * dt

        ::continue::
    end
end


return MovementSystem

