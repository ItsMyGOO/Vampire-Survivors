---
--- Created by echo.
--- DateTime: 2025/12/21 12:18
---
print("lua bootstrap start")

local GlobalGuard = require("Dev.GlobalGuard")
GlobalGuard.enable()

require("Game.Skill.Fireball")
require("Game.Buff.BuffSystem")
require("Game.Buff.Impl.FireBoost")
