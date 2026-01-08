---
--- Created by echo.
--- DateTime: 2025/12/29 10:55
---
local RF = require("Presentation.render_flags")

---@class LuaRenderBridge
local LuaRenderBridge = {}

---@param world World
---@return CS.LuaRenderItem[]
function LuaRenderBridge.Collect(world)
    local list = {}
    local C = _G.ComponentRegistry

    ---@type table<integer, PositionComponent>
    local positions = world:GetComponentOfType(C.Position)
    ---@type table<integer, SpriteKeyComponent>
    local spriteKeys = world:GetComponentOfType(C.SpriteKey)
    ---@type table<integer, MoveIntentComponent>
    local intents = world:GetComponentOfType(C.MoveIntent)
    local rotations = world:GetComponentOfType(C.Rotation)


    for eid, pos in pairs(positions) do
        local spriteKey = spriteKeys[eid]
        local intent    = intents[eid]
        local rotation  = rotations[eid]

        local item      = CS.LuaRenderItem() -- default(struct)
        item.eid        = eid
        item.posX       = pos and pos.x or 0
        item.posY       = pos and pos.y or 0
        item.posZ       = pos and pos.z or 0

        local flags     = RF.None -- ★ 必须先初始化

        if intent then
            item.dirX = intent.dirX
            flags     = flags | RF.FlipX
        end

        if rotation then
            item.rotation = rotation.rotation
            flags         = flags | RF.Rotation
        end
        item.flags      = flags
        
        item.sheet      = spriteKey.sheet
        item.spriteKey  = spriteKey.key

        list[#list + 1] = item
    end

    return list
end

return LuaRenderBridge
