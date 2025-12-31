---
--- Created by echo.
--- DateTime: 2025/12/29 19:36
---
local ComponentRegistry      = {}

--- transform
ComponentRegistry.Position   = require("ecs.components.position")
ComponentRegistry.Velocity   = require("ecs.components.velocity")

ComponentRegistry.Steering   = require("ecs.components.steering")
ComponentRegistry.Seek     = require("ecs.components.seek")
ComponentRegistry.Separation = require("ecs.components.separation")
ComponentRegistry.MoveIntent     = require("ecs.components.move_intent")


--- animation
ComponentRegistry.Animation        = require("ecs.components.animation")
ComponentRegistry.SpriteKey        = require("ecs.components.sprite_key")
ComponentRegistry.AnimationCommand = require("ecs.components.animation_command")

--- battle
ComponentRegistry.PlayerTag        = require("ecs.components.player_tag")

return ComponentRegistry
