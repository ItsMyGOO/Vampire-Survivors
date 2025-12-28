---
--- Created by echo.
--- DateTime: 2025/12/28 17:28
---
local BaseSystem = require("ecs.base_system")

---@class EnemyPerceptionSystem : BaseSystem
local EnemyPerceptionSystem = {}
EnemyPerceptionSystem.__index = EnemyPerceptionSystem

function EnemyPerceptionSystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, EnemyPerceptionSystem)
end

---@param world World
function EnemyPerceptionSystem:start(world)
    BaseSystem.start(self, world)

    -- 获取需要的组件
    local Chase = require("ecs.components.chase")

    self.chase = world:GetComponentOfType(Chase)
end

function EnemyPerceptionSystem:update(dt)
    for eid, chase in pairs(self.chase) do
        chase.target_eid = self.world.player_eid
    end
end

function EnemyPerceptionSystem:shutdown()
    -- 清理引用
    self.transforms = nil
end

return EnemyPerceptionSystem
