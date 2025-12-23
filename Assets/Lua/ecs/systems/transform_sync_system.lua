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

--- 创建新的Transform同步系统实例
---@override
---@return TransformSyncSystem
function TransformSyncSystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, TransformSyncSystem)
end

--- 系统启动方法，初始化Transform组件引用
---@override
--- @param world table ECS世界实例
function TransformSyncSystem:start(world)
    BaseSystem.start(self, world)

    local Transform = require("ecs.components.transform")
    self.transforms = world:GetComponentOfType(Transform)
end

--- 系统更新方法，同步ECS Transform到Unity Transform
---@override
--- @param dt number 帧时间间隔
function TransformSyncSystem:update(dt)
    for _, trans in pairs(self.transforms) do
        if trans.go and trans.dirty then
            trans.go.transform.position =
                CS.UnityEngine.Vector3(trans.x, trans.y, trans.z)
            trans.dirty = false
        end
    end
end

--- 系统关闭方法，清理引用
---@override
function TransformSyncSystem:shutdown()
    self.transforms = nil
end

return TransformSyncSystem
