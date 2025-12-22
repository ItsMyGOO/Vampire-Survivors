---
--- Created by echo.
--- DateTime: 2025/12/21 12:18
---
print("lua bootstrap start")

FireBall = require('Game.Skill.fireball')
BuffSystem = require('Game.Buff.buff_system')
FireBoost = require('Game.Buff.Impl.fire_boost')

require("Test.movement_system_test")()
