---
--- Created by echo.
--- DateTime: 2025/12/21 11:05
---
local buff = {}
buff.__index = buff

function buff.new(hooks)
    return setmetatable({
        hooks = hooks
    }, buff)
end

return buff
