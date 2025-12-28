---
--- Created by echo.
--- DateTime: 2025/12/28 22:47
---
-- AnimationCommandSystem.lua
local BaseSystem = require("ecs.base_system")
local Animation = require("ecs.components.animation")
local AnimationCommand = require("ecs.components.animation_command")

---@class AnimationCommandSystem: BaseSystem
AnimationCommandSystem = {
    ---@type table<integer, AnimationComponent>?
    animations = nil,
    ---@type table<integer, AnimationCommandComponent>?
    animation_command = nil
}
AnimationCommandSystem.__index = AnimationCommandSystem

function AnimationCommandSystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, AnimationCommandSystem)
end

---@param world World
function AnimationCommandSystem:start(world)
    BaseSystem.start(self, world)

    self.animations = world:GetComponentOfType(Animation)
    self.animation_command = world:GetComponentOfType(AnimationCommand)
end

function AnimationCommandSystem:update(dt)
    if (self.animations and self.animation_command) then
        for eid, command in pairs(self.animation_command) do
            local anim = self.animations[eid]
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

            self.world:RemoveComponent(eid, AnimationCommand)

            ::continue::
        end
    end
end

function AnimationCommandSystem:shutdown()
    self.animations = nil
end

return AnimationCommandSystem
