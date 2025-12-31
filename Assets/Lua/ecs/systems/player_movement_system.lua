---
--- Created by echo.
--- DateTime: 2025/12/31 23:07
---
---@class PlayerMovementSystem
local PlayerMovementSystem={}
PlayerMovementSystem.__index = PlayerMovementSystem

function PlayerMovementSystem:update(world, dt)
    local C = _G.ComponentRegistry

    local velocities = world:GetComponentOfType(C.Velocity)
    local intents    = world:GetComponentOfType(C.MoveIntent)

    for eid, intent in pairs(intents) do
        local vel = velocities[eid]
        if not vel then goto continue end

        vel.x = intent.dirX * 2
        vel.y = intent.dirY * 2

        ::continue::
    end
end

return PlayerMovementSystem