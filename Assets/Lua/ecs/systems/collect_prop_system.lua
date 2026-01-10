---
--- Created by echo.
--- DateTime: 2026/1/10 20:03
---
---@class CollectPropSystem
local CollectPropSystem = {}
CollectPropSystem.__index = CollectPropSystem
local C = _G.ComponentRegistry

---@param world World
function CollectPropSystem:update(world, dt)
    local colliders = world:GetComponentOfType(C.Collider)
    local props     = world:GetComponentOfType(C.Prop)
    local player    = world.player

    local playerCol = colliders[world.player_eid]

    if not playerCol or not player then
        return
    end

    for eid, prop in pairs(props) do
        local pCol    = colliders[eid]

        -- 圆形碰撞检测
        local dx      = playerCol.x - pCol.x
        local dy      = playerCol.y - pCol.y
        local r       = playerCol.radius + pCol.radius
        local inRange = (dx * dx + dy * dy) <= r * r

        -- 不在范围
        if not inRange then
            goto continue
        end

        -- ===== 拾取成功 =====
        player:CollectProp(prop)

        -- 移除道具实体
        world:DestroyEntity(eid)

        ::continue::
    end
end

return CollectPropSystem
