---
--- Created by echo.
--- DateTime: 2025/12/21 11:04
---
local buff_system = require("Game.Buff.BuffSystem")

local DamageCalc = {}

function DamageCalc.execute(ctx)
    buff_system:trigger("on_damage_calc", ctx)
end

return DamageCalc
