---
--- Created by echo.
--- DateTime: 2025/12/22 16:23
---
-- test/movement_system_test.lua
function test_movement_applies_velocity()
    local world = require("ecs.world")()
    local Transform = require("ecs.components.transform")
    local Velocity = require("ecs.components.velocity")

    local eid = world:AddEntity()
    world:AddComponent(eid, Transform, {x=0,y=0,z=0})
    world:AddComponent(eid, Velocity, {x=1,y=0,z=0})

    local sys = require("ecs.systems.movement_system")()
    sys:start(world)

    sys:update(1.0)

    local t = world:GetComponent(eid, Transform)
    assert(t.x == 1.0, "X should be moved by velocity")
end