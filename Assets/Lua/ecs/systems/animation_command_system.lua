---
--- Created by echo.
--- DateTime: 2025/12/28 22:47
---
-- AnimationCommandSystem.lua
---@class AnimationCommandSystem
AnimationCommandSystem = {
}
AnimationCommandSystem.__index = AnimationCommandSystem

---@param world World
---@param dt number
function AnimationCommandSystem:update(world, dt)
    ---@type table<integer, AnimationComponent>
    local animations = world:GetComponentOfType(_G.ComponentRegistry.Animation)
    ---@type table<integer, AnimationCommandComponent>
    local animation_commands = world:GetComponentOfType(_G.ComponentRegistry.AnimationCommand)

    for eid, command in pairs(animation_commands) do
        local anim = animations[eid]
        if (not anim) then
            goto continue
        end

        if command.play_animation_name and anim.clipId ~= command.play_animation_name then
            anim.clipId = command.play_animation_name
            anim.frame = 1
            anim.time = 0
            anim.playing = true
        end

        if command.stop then
            anim.playing = false
        end

        world:RemoveComponent(eid, _G.ComponentRegistry.AnimationCommand)

        ::continue::
    end
end

return AnimationCommandSystem
