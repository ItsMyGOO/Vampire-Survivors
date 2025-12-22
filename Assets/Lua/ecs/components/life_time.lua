---
--- Created by echo.
--- DateTime: 2025/12/22 15:29
---
-- 生命周期组件（用于延迟销毁）
-- components/life_time.lua

return {
    ---@type number 剩余存活时间
    remaining = 1.0,
    ---@type function|nil 过期时的回调函数
    onExpire = nil,
}
