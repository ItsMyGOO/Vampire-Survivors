---
--- Created by echo.
--- DateTime: 2025/12/21 21:51
---
local Component = require("ecs.component")

local Velocity = Component.Register("Velocity", {
    x = 0, y = 0, z = 0,
})

return Velocity