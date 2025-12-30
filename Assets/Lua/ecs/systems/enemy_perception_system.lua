---
--- Created by echo.
--- DateTime: 2025/12/28 17:28
---
---@class EnemyPerceptionSystem
local EnemyPerceptionSystem = {}
EnemyPerceptionSystem.__index = EnemyPerceptionSystem

---@param world World
---@param dt number
function EnemyPerceptionSystem:update(world, dt)
    ---@type table<integer, ChaseComponent>
    local chases = world:GetComponentOfType(_G.ComponentRegistry.Chase)

    for eid, chase in pairs(chases) do
        chase.target_eid = world.player_eid
    end
end

return EnemyPerceptionSystem
