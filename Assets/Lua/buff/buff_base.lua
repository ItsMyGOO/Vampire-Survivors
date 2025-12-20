---
--- Created by echo.
--- DateTime: 2025/12/20 17:26
---

local BuffBase = {}
BuffBase.__index = BuffBase

function BuffBase:new()
    local o = {}
    setmetatable(o, self)
    return o
end

function BuffBase:ModifyDamage(ctx)
end

return BuffBase