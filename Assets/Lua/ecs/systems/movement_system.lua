---
--- Created by echo.
--- DateTime: 2025/12/21 21:46
---
-- ecs/systems/movement_system.lua
---@class MovementSystem
local MovementSystem = {
    world = nil,
    _transforms = nil,
    _velocities = nil
}

function MovementSystem:start(world)
    self.world = assert(world, "World is required")

    -- 使用 schema 获取真实名称
    local Transform = require("ecs.components.transform")
    local Velocity = require("ecs.components.velocity")

    self._transforms = self.world.components[self.world:getSchemaName(Transform)]
    self._velocities = self.world.components[self.world:getSchemaName(Velocity)]
end

function MovementSystem:update(dt)
    if not self._transforms or not self._velocities then return end

    local transforms = self._transforms
    local velocities = self._velocities

    for eid, vel in pairs(velocities) do
        local trans = transforms[eid]
        if trans and vel.active ~= false then
            trans.x = (trans.x or 0) + (vel.x or 0) * dt
            trans.y = (trans.y or 0) + (vel.y or 0) * dt
            trans.z = (trans.z or 0) + (vel.z or 0) * dt

            -- 同步到 Unity（可选）
            if trans.go then
                trans.go.transform.position = CS.UnityEngine.Vector3(trans.x, trans.y, trans.z)
            end
        end
    end
end

return MovementSystem
