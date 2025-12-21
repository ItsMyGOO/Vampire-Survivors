---
--- Created by echo.
--- DateTime: 2025/12/21 21:42
---
local Component = require("ecs.component")

local Transform = Component.Register("Transform", {
    x = 0, y = 0, z = 0,
    go = nil  -- 绑定的 Unity GameObject
})

return Transform