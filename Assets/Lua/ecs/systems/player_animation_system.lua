---
--- Created by echo.
--- DateTime: 2026/1/9 18:22
---
local PlayerAnimationSystem = {}
PlayerAnimationSystem.__index = PlayerAnimationSystem

---@param world World
function PlayerAnimationSystem:update(world, dt)
    local C = _G.ComponentRegistry

    local eid = world.player_eid
    local input = _G.InputData

    local animations = world:GetComponentOfType(C.Animation)
    local anim = animations[eid]

    local hasInput = input.hori ~= 0 or input.vert ~= 0
    local desiredState = hasInput and "Run" or "Idle"

    if anim.state ~= desiredState then
        anim.state = desiredState
        anim.clipId = desiredState
        anim.frame = 1
        anim.time = 0
        anim.playing = true
    end
end

return PlayerAnimationSystem
