---
--- Created by echo.
--- DateTime: 2025/12/22 15:29
---
-- 生命周期组件（用于延迟销毁）
local Component = require("ecs.component")

local LifeTime = Component.Register("LifeTime", {
    remaining = 1.0,
    onExpire = nil,
})
return LifeTime