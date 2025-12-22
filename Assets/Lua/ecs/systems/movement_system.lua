---
--- Created by echo.
--- DateTime: 2025/12/21 21:46
---

-- systems/movement_system.lua
-- 批量移动系统：处理所有 Velocity + Transform 实体
-- 设计原则：高缓存命中、零 GC、类型安全

local Transform = require("ecs.components.transform")
local Velocity = require("ecs.components.velocity")

---@class MovementSystem
---@field public world World
---@field private _transforms table<number, Transform>
---@field private _velocities table<number, Velocity>
local MovementSystem = {
    _transforms = nil,
    _velocities = nil,
    world = nil
}

--- 启动系统（由 World 调用）
-- @param self
-- @param world World
function MovementSystem:start(world)
    self.world = assert(world, "World is required")
    self._transforms = world.components[Transform.__name]
    self._velocities = world.components[Velocity.__name]
end

--- 每帧更新
-- @param self
-- @param dt number
function MovementSystem:update(dt)
    if not self._transforms or not self._velocities then
        return
    end

    local transforms = self._transforms
    local velocities = self._velocities

    for eid, vel in pairs(velocities) do
        local trans = transforms[eid]
        if trans and vel.active ~= false then
            trans.x = (trans.x or 0) + (vel.x or 0) * dt
            trans.y = (trans.y or 0) + (vel.y or 0) * dt
            trans.z = (trans.z or 0) + (vel.z or 0) * dt

            if trans.go then
                trans.go.transform.position = CS.UnityEngine.Vector3(trans.x, trans.y, trans.z)
            end
        end
    end
end

-- 支持作为工厂调用：local sys = MovementSystem()
-- 一般不需要，除非你要创建多个独立移动上下文
-- setmetatable(MovementSystem, {
--     __call = function() return setmetatable({}, { __index = MovementSystem }) end
-- })

return MovementSystem
