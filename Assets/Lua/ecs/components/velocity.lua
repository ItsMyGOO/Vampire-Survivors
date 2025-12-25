---
--- Created by echo.
--- DateTime: 2025/12/21 21:51
---
-- components/velocity.lua
local M = {
    x = 0.0,
    y = 0.0,
    z = 0.0,
    speed = 1,
    active = true
}

return setmetatable(M, { __name = "Velocity" })
