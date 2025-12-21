---
--- Created by echo.
--- DateTime: 2025/12/21 11:04
---
local BuffEvent = require("Game.Common.BuffEvent")

local DamageCalc = {}

function DamageCalc:execute(ctx)
    ctx.damage = ctx.base_damage
    ctx.buff_system:trigger(BuffEvent.DAMAGE_CALC, ctx)
end

return DamageCalc
