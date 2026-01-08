---
--- Created by echo.
--- DateTime: 2025/12/29 10:55
---
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

    for eid, pos in pairs(positions) do
        local spriteKey = spriteKeys[eid]
        local intent    = intents[eid]

        local item      = CS.LuaRenderItem() -- default(struct)
        item.eid        = eid
        item.posX       = pos and pos.x or 0
        item.posY       = pos and pos.y or 0
        item.posZ       = pos and pos.z or 0
        item.dirX       = intent and intent.dirX
        item.sheet      = spriteKey.sheet
        item.spriteKey  = spriteKey.key

        list[#list + 1] = item
    end

    return list
end

return LuaRenderBridge
