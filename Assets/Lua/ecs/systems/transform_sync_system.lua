---
--- Created by echo.
--- DateTime: 2025/12/23 21:17
---
--- Transform同步系统
--- 将ECS中的Transform组件同步到Unity引擎的Transform
local BaseSystem = require("ecs.base_system")

---@class TransformSyncSystem : BaseSystem
local TransformSyncSystem = {}
TransformSyncSystem.__index = TransformSyncSystem

---@return TransformSyncSystem
function TransformSyncSystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, TransformSyncSystem)
end

---@param world World
function TransformSyncSystem:start(world)
    BaseSystem.start(self, world)

    local Transform = require("ecs.components.transform")
    self.transforms = world:GetComponentOfType(Transform)
end

function TransformSyncSystem:update(dt)
    for _, trans in pairs(self.transforms) do
        if trans.go and trans.dirty then
            trans.go.transform.position =
                CS.UnityEngine.Vector3(trans.x, trans.z, 0)
            trans.dirty = false
        end
    end
end

function TransformSyncSystem:shutdown()
    self.transforms = nil
end

return TransformSyncSystem
