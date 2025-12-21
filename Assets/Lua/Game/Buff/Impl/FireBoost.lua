---
--- Created by echo.
--- DateTime: 2025/12/21 11:06
---
local Buff      = require("Game.Buff.Buff")
local BuffEvent = require("Game.Common.BuffEvent")

local FireBoost = {}

function FireBoost.new()
    return Buff.new({
        [BuffEvent.DAMAGE_CALC] = function(self, ctx)
            if ctx:has_tag("fire") then
                ctx.damage = ctx.damage * 1.5
            end
        end
    })
end

return FireBoost
