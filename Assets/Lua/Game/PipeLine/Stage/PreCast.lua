---
--- Created by echo.
--- DateTime: 2025/12/21 11:04
---
local BuffEvent = require("Game.Common.BuffEvent")
local buff_system = require("Game.Buff.BuffSystem")

local PreCast = {}

function PreCast.execute(ctx)
    buff_system:trigger(BuffEvent.PRE_CAST, ctx)
end

return PreCast
