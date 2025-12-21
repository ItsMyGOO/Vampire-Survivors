---
--- Created by echo.
--- DateTime: 2025/12/21 11:05
---
local Buff = {}
Buff.__index = Buff

function Buff.new(hooks)
    return setmetatable({
        hooks = hooks
    }, Buff)
end

return Buff
