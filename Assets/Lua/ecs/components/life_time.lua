---
--- Created by echo.
--- DateTime: 2025/12/22 15:29
---
-- 生命周期组件（用于延迟销毁）
-- components/life_time.lua

--- @class LifeTimeComponent: table
--- @field remaining number 剩余生命周期，单位秒
--- @field onExpire function? 生命周期结束时的回调函数
local M = {
    remaining = 1.0,
    onExpire = nil,
}

return M
