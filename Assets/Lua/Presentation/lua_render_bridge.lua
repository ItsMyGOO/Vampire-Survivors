---
--- Created by echo.
--- DateTime: 2025/12/29 10:55
---
LuaRenderBridge = {}

local Transform = require("ecs.components.transform")
local Velocity = require("ecs.components.velocity")
local SpriteKey = require("ecs.components.sprite_key")
local RenderHandle = require("ecs.components.render_handler")

---@param world World
---@return CS.LuaRenderItem[]
function LuaRenderBridge:Collect(world)
    local list = {}

    ---@type table<integer, RenderHandlerComponent>
    local renders = world:GetComponentOfType(RenderHandle)
    ---@type table<integer, TransformComponent>
    local transforms = world:GetComponentOfType(Transform)
    ---@type table<integer, SpriteKeyComponent>
    local spriteKeys = world:GetComponentOfType(SpriteKey)
    ---@type table<integer, VelocityComponent>
    local velocities = world:GetComponentOfType(Velocity)

    for eid, render in pairs(renders) do
        local trans     = transforms[eid]
        local spriteKey = spriteKeys[eid]
        local velocity  = velocities[eid]

        local item      = CS.LuaRenderItem() -- default(struct)
        item.transform  = render.transform
        item.renderer   = render.renderer
        item.pos        = CS.UnityEngine.Vector3(
            trans and trans.x or 0,
            trans and trans.y or 0,
            trans and trans.z or 0
        )
        item.velocityX  = velocity and velocity.x or 0
        item.sheet      = spriteKey.sheet
        item.spriteKey  = spriteKey.key

        list[#list + 1] = item
    end

    return list
end
