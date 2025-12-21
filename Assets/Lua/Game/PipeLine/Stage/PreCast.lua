---
--- Created by echo.
--- DateTime: 2025/12/21 11:04
---
local BuffEvent = require("Game.Common.BuffEvent")

local PreCast = {}

function PreCast:execute(ctx)
    ctx.buff_system:trigger(BuffEvent.PRE_CAST, ctx)
end

return PreCast
