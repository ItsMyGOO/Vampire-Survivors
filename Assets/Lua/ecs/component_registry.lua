---
--- Created by echo.
--- DateTime: 2025/12/29 19:36
---
local ComponentRegistry            = {}

--- transform
ComponentRegistry.Position         = require("ecs.components.position")
ComponentRegistry.Velocity         = require("ecs.components.velocity")
ComponentRegistry.Rotation         = require("ecs.components.rotation")
ComponentRegistry.MoveIntent       = require("ecs.components.move_intent")
ComponentRegistry.Seek             = require("ecs.components.seek")

--- animation
ComponentRegistry.Animation        = require("ecs.components.animation")
ComponentRegistry.SpriteKey        = require("ecs.components.sprite_key")
ComponentRegistry.AnimationCommand = require("ecs.components.animation_command")

--- battle
ComponentRegistry.PlayerTag        = require("ecs.components.player_tag")
ComponentRegistry.EnemyTag         = require("ecs.components.enemy_tag")
-- 一把武器一个 slot
ComponentRegistry.WeaponSlots      = require("ecs.components.weapon_slots")

ComponentRegistry.Projectile       = {
    damage = 0,
    owner = -1,
    lifetime = 2
}

-- 跟随型 / 环绕型武器
ComponentRegistry.Orbit            = {
    owner = -1,
    radius = 1,
    angle = 0,
    angularSpeed = 0
}

ComponentRegistry.Damage           = {
    value = 0
}
return ComponentRegistry
