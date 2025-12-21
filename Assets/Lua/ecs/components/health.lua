---
--- Created by echo.
--- DateTime: 2025/12/21 21:52
---
local Component = require("ecs.component")

local Health = Component.Register("Velocity", {
    hp = 0
})

return Health
