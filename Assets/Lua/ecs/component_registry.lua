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

--
ComponentRegistry.WeaponSlots      = require("ecs.components.weapon_slots")
ComponentRegistry.Projectile       = require("ecs.components.projectile")
ComponentRegistry.Orbit            = require("ecs.components.orbit")

ComponentRegistry.DamageSource     = require("ecs.components.damage_source")
ComponentRegistry.Collider         = require("ecs.components.collider")
ComponentRegistry.Knockback        = require("ecs.components.knockback")
ComponentRegistry.Health           = require("ecs.components.health")

return ComponentRegistry
