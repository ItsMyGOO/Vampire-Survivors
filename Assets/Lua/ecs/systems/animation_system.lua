---
--- Created by echo.
--- DateTime: 2025/12/28 19:45
---
-- AnimationSystem.lua
local AnimationConfigHandler = require("ConfigHandler.anim_config_handler")

---@class AnimationSystem
AnimationSystem = {}
AnimationSystem.__index = AnimationSystem

---@param world World
---@param dt number
function AnimationSystem:update(world, dt)
    ---@type table<integer, AnimationComponent>
    local animations = world:GetComponentOfType(_G.ComponentRegistry.Animation)
    ---@type table<integer, SpriteKeyComponent>
    local sprite_keys = world:GetComponentOfType(_G.ComponentRegistry.SpriteKey)

    for eid, anim in pairs(animations) do
        local spriteKeyComp = sprite_keys[eid]

        if not spriteKeyComp or not anim.playing or not anim.clipSetId or not anim.clipId then
            goto continue
        end

        local sheet, clip = AnimationConfigHandler.GetConfig(anim.clipSetId, anim.clipId)
        if not sheet or not clip then
            goto continue
        end

        anim.time = anim.time + dt
        local frameTime = 1 / clip.fps

        while anim.time >= frameTime do
            anim.time = anim.time - frameTime
            anim.frame = anim.frame + 1

            -- 播放到末尾
            if anim.frame > #clip.frames then
                if clip.loop then
                    anim.frame = 1
                else
                    -- 一次性动画结束
                    anim.playing = false
                    -- ★★★ 核心：自动回退 ★★★
                    if anim.defaultState and anim.state ~= anim.defaultState then
                        print(anim.defaultState)
                        anim.state = anim.defaultState
                        anim.clipId = anim.defaultState
                        anim.frame = 1
                        anim.time = 0
                        anim.playing = true
                    else
                        anim.frame = #clip.frames
                    end

                    break
                end
            end
        end

        spriteKeyComp.sheet = sheet
        spriteKeyComp.key = clip.frames[anim.frame]

        ::continue::
    end
end

return AnimationSystem
