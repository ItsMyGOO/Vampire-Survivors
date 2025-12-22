---
--- Created by echo.
--- DateTime: 2025/12/22 15:20
---
-- 表示一个正在下落的子弹
local Component = require("ecs.component")

local Drop = Component.Register("BulletRainDrop", {
    speed = 3.0,
    isFalling = true,
    targetY = 0,
})
return Drop
