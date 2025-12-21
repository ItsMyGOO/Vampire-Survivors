---
--- Created by echo.
--- DateTime: 2025/12/21 11:06
---
local Buff = require("Game.Buff.Buff")

local FireBoost = {}

function FireBoost.new()
    return Buff.new({
        on_damage_calc = function(self, ctx)
            for _, tag in ipairs(ctx.tags) do
                if tag == "fire" then
                    ctx.damage = ctx.damage * 1.5
                    print("fire boost applied")
                    break
                end
            end
        end
    })
end

return FireBoost
