---
--- Created by echo.
--- DateTime: 2025/12/29 10:55
---
LuaRenderBridge = {}

local ComponentRegistry = _G.ComponentRegistry

---@param world World
---@return CS.LuaRenderItem[]
function LuaRenderBridge:Collect(world)
    local list = {}

    ---@type table<integer, AnimationComponent>
    local animations = world:GetComponentOfType(ComponentRegistry.Animation)
    ---@type table<integer, PositionComponent>
    local positions = world:GetComponentOfType(ComponentRegistry.Position)
    ---@type table<integer, SpriteKeyComponent>
    local spriteKeys = world:GetComponentOfType(ComponentRegistry.SpriteKey)
    ---@type table<integer, VelocityComponent>
    local velocities = world:GetComponentOfType(ComponentRegistry.Velocity)

    for eid, anim in pairs(animations) do
        local trans     = positions[eid]
        local velocity  = velocities[eid]
        local spriteKey = spriteKeys[eid]

        local item      = CS.LuaRenderItem() -- default(struct)
        item.eid        = eid
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
