---
--- Created by echo.
--- DateTime: 2025/12/29 15:04
---
local AnimationDB = require("Data.animation_db")

local AnimConfigHandler = {}

---@param setId string?
---@param clipId string?
---@return AnimationConfig?
function AnimConfigHandler.GetConfig(setId, clipId)
    ---@type PlayerAnimations
    local clipSet = AnimationDB[setId]
    if (not clipSet) then
        return nil
    end

    ---@type AnimationConfig
    local clip = clipSet[clipId]
    if (not clip) then
        return nil
    end

    if (not clip.frames and clip.name and clip.frame_count) then
        local frames = {}
        for i = 1, clip.frame_count do
            frames[i] = clip.name .. " " .. i - 1
        end
        clip.frames = frames
    end

    return clip
end

return AnimConfigHandler
