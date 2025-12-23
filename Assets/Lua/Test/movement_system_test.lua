---
--- Created by echo.
--- DateTime: 2025/12/22 16:23
---
-- test/movement_system_test.lua
local function test_movement_applies_velocity()
    -- Arrange
    local World = require("ecs.world")
    local Transform = require("ecs.components.transform")
    local Velocity = require("ecs.components.velocity")

    local world = setmetatable({}, { __index = World }) -- 模拟 world 实例
    world.eventBus = nil                                -- 如果系统依赖 eventBus，mock 它

    local eid = world:AddEntity()
    world:AddComponent(eid, Transform, { x = 0, y = 0, z = 0 })
    world:AddComponent(eid, Velocity, { x = 1, y = 0, z = 0 })

    local movement = require("ecs.systems.movement_system").new()
    movement:start(world)
    -- Act
    movement:update(1.0) -- 移动 1 秒

    -- Assert
    local t = world:GetComponent(eid, Transform)
    assert(t ~= nil, "Transform component should exist")
    print("Final X:", t.x) -- 调试输出
    assert(math.abs(t.x - 1.0) < 0.001, "X should be moved by velocity, got " .. tostring(t.x))
end

return test_movement_applies_velocity
