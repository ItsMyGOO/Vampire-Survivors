---
--- Created by echo.
--- DateTime: 2025/12/29 10:55
---
LuaRenderBridge = {}

local Transform = require("ecs.components.transform")
local SpriteKey = require("ecs.components.sprite_key")
local RenderHandle = require("ecs.components.render_handler")

---@class LuaRenderItem
local LuaRenderItem = CS.LuaRenderItem
---@param world World
---@return LuaRenderItem[]
function LuaRenderBridge:Collect(world)
    local list = {}

    ---@type table<integer, RenderHandlerComponent>
    local renders = world:GetComponentOfType(RenderHandle)
    ---@type table<integer, TransformComponent>
    local transforms = world:GetComponentOfType(Transform)
    ---@type table<integer, SpriteKeyComponent>
    local spriteKeys = world:GetComponentOfType(SpriteKey)

    for eid, render in pairs(renders) do
        local trans     = transforms[eid]
        local spriteKey = spriteKeys[eid]

        local item      = LuaRenderItem() -- default(struct)
        item.transform  = render.transform
        item.renderer   = render.renderer
        item.x          = trans and trans.x or 0
        item.y          = trans and trans.y or 0
        item.z          = trans and trans.z or 0
        item.sheet      = spriteKey.sheet
        item.spriteKey  = spriteKey.key

        list[#list + 1] = item
    end

    return list
end
