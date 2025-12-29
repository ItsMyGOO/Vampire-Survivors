---
--- Created by echo.
--- DateTime: 2025/12/28 19:45
---
-- AnimationSystem.lua
local BaseSystem = require("ecs.base_system")
local AnimationDB = require("Data.animation_db")

---@class AnimationSystem: BaseSystem
AnimationSystem = {
    ---@type table<number,AnimationComponent>?
    animations = nil,
    ---@type table<number,SpriteKeyComponent>?
    sprite_key = nil,
}
AnimationSystem.__index = AnimationSystem

function AnimationSystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, AnimationSystem)
end

---@param world World
function AnimationSystem:start(world)
    BaseSystem.start(self, world)

    local Animation = require("ecs.components.animation")
    local SpriteKey = require("ecs.components.sprite_key")

    self.animations = world:GetComponentOfType(Animation)
    self.sprite_key = world:GetComponentOfType(SpriteKey)
end

function AnimationSystem:update(dt)
    if (self.animations and self.sprite_key) then
        for eid, anim in pairs(self.animations) do
            local spriteKeyComp = self.sprite_key[eid]
            if not spriteKeyComp or not anim.playing or not anim.clipId then
                goto continue
            end

            ---@type PlayerAnimations
            local clipSet = AnimationDB[anim.clipSetId]
            ---@type AnimationConfig
            local clip = clipSet and clipSet[anim.clipId]
            if not clip then
                goto continue
            end

            anim.time = anim.time + dt
            local frameTime = 1 / clip.fps

            while anim.time >= frameTime do
                anim.time = anim.time - frameTime
                anim.frame = anim.frame + 1

                if anim.frame > #clip.frames then
                    if clip.loop then
                        anim.frame = 1
                    else
                        anim.frame = #clip.frames
                        anim.playing = false
                    end
                end
            end

            spriteKeyComp.sheet = clip.sheet
            spriteKeyComp.key = clip.frames[anim.frame]

            ::continue::
        end
    end
end

function AnimationSystem:shutdown()
    self.animations = nil
    self.sprite_key = nil
end

return AnimationSystem
