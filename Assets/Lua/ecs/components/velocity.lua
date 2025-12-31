---
--- Created by echo.
--- DateTime: 2025/12/21 21:51
---
-- components/velocity.lua

--- @class VelocityComponent: table
--- @field x number X轴速度分量
--- @field y number Y轴速度分量
--- @field z number Z轴速度分量
--- @field speed number 移动速度
--- @field active boolean 是否激活移动
local M = {
    x = 0.0,
    y = 0.0,
    z = 0.0,
    active = true
}

return M
