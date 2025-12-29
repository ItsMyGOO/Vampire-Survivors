---
--- Created by echo.
--- DateTime: 2025/12/28 21:35
---
print("测试animation")

local World = require("ecs.world")
MainWorld = World

MainWorld:AddSystem(require("ecs.systems.animation_command_system").new())
MainWorld:AddSystem(require("ecs.systems.animation_system").new())

local AnimationComponent = require("ecs.components.animation")
local SpriteKeyComponent = require("ecs.components.sprite_key")
local AnimationCommand = require("ecs.components.animation_command")
local RenderHandler = require("ecs.components.render_handler")

--- @param transform CS.UnityEngine.Transform
--- @param spriteRenderer CS.UnityEngine.SpriteRenderer
function CreateTestEntity(transform, spriteRenderer)
    local eid = MainWorld:AddEntity()

    MainWorld:AddComponent(eid, AnimationComponent, {
        clipSetId = "Player1"
    })
    MainWorld:AddComponent(eid, SpriteKeyComponent)
    MainWorld:AddComponent(eid, AnimationCommand, {
        play_animation_name = "Run"
    })
    MainWorld:AddComponent(eid, RenderHandler, {
        transform = transform,
        renderer = spriteRenderer,
    })

    return eid
end

-- 全局暴露给 C#
_G.UpdateGame = function(dt)
    MainWorld:UpdateSystems(dt)
end
_G.SpawnPlayer = SpawnPlayer
