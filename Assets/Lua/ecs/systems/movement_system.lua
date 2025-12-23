---
--- Created by echo.
--- DateTime: 2025/12/21 21:46
---
--- 处理实体移动的系统
--- 继承自 BaseSystem，负责根据速度组件更新实体位置
local BaseSystem = require("ecs.base_system")

---@class MovementSystem : BaseSystem
local MovementSystem = {}
MovementSystem.__index = MovementSystem

--- 创建新的移动系统实例
---@override
---@return MovementSystem
function MovementSystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, MovementSystem)
end

--- 系统启动方法，初始化移动系统所需组件
---@override
--- @param world table ECS世界实例
function MovementSystem:start(world)
    BaseSystem.start(self, world)

    local Transform = require("ecs.components.transform")
    local Velocity  = require("ecs.components.velocity")

    self.transforms = world:GetComponentOfType(Transform)
    self.velocities = world:GetComponentOfType(Velocity)
end

--- 系统更新方法，根据速度更新实体位置
--- @override
--- @param dt number 帧时间间隔
function MovementSystem:update(dt)
    for eid, vel in pairs(self.velocities) do
        local trans = self.transforms[eid]
        if trans and vel.active ~= false then
            trans.x = trans.x + vel.x * dt
            trans.y = trans.y + vel.y * dt
            trans.z = trans.z + vel.z * dt
            trans.dirty = true
        end
    end
end

--- 系统关闭方法，清理引用
---@override
function MovementSystem:shutdown()
    self.transforms = nil
    self.velocities = nil
end

return MovementSystem
