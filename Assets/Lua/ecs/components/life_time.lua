---
--- Created by echo.
--- DateTime: 2025/12/22 15:29
---
-- 生命周期组件（用于延迟销毁）
-- components/life_time.lua
local M = {
    remaining = 1.0,
    onExpire = nil,
}

return setmetatable(M, { __name = "LifeTime" })
