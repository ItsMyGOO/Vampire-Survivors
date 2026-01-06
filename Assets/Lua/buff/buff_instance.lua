---
--- Created by echo.
--- DateTime: 2026/1/5 19:46
---
-- BuffInstance.lua
local BuffInstance = {}
BuffInstance.__index = BuffInstance

function BuffInstance.new(def)
    return setmetatable({
        def = def,
        id = def.id,
        stack = 1,
    }, BuffInstance)
end

return BuffInstance
