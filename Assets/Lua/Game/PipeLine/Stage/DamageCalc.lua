---
--- Created by echo.
--- DateTime: 2025/12/21 11:04
---
local buff_system = require("Game.Buff.BuffSystem")
local BuffEvent = require("Game.Common.BuffEvent")

local DamageCalc = {}

function DamageCalc.execute(ctx)
    buff_system:trigger(BuffEvent.DAMAGE_CALC, ctx)
end

return DamageCalc
