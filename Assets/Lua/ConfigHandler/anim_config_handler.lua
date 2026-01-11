---
--- Created by echo.
--- DateTime: 2025/12/29 15:04
---
local AnimationDB = require("Data.animation_db")

local AnimConfigHandler = {}

---@param setId string
---@param clipId string
---@return string? sheet
---@return AnimationConfig? clip
function AnimConfigHandler.GetConfig(setId, clipId)
    local clipSet = AnimationDB[setId]
    if not clipSet then
        return nil
    end

    local clip = clipSet[clipId]
    if not clip then
        return nil
    end

    -- lazy build frames
    if not clip.frames and clip.name and clip.frame_count then
        local frames = {}

        if clip.frame_count == 1 then
            frames[1] = clip.name
        else
            for i = 1, clip.frame_count do
                frames[i] = clip.name .. " " .. (i - 1)
            end
        end

        clip.frames = frames
    end

    return clipSet.sheet, clip
end

return AnimConfigHandler
