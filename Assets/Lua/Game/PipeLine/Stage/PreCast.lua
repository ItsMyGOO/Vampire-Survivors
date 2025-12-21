---
--- Created by echo.
--- DateTime: 2025/12/21 11:04
---
local buff_system = require("Game.Buff.BuffSystem")

local PreCast = {}

function PreCast.execute(ctx)
    buff_system:trigger("on_pre_cast", ctx)
end

return PreCast
