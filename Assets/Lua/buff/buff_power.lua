---
--- Created by echo.
--- DateTime: 2025/12/20 17:27
---

local BuffBase = require("buff.buff_base")

local BuffPower = setmetatable({}, { __index = BuffBase })
BuffPower.__index = BuffPower

function BuffPower:new()
    local o = BuffBase.new(self)
    return o
end

function BuffPower:ModifyDamage(ctx)
    ctx.finalDamage = ctx.finalDamage * 1.2
end

return BuffPower