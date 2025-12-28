---
--- Created by echo.
--- DateTime: 2025/12/22 16:28
---
-- components/falling_body.lua
-- 表示任何会下落的物体（子弹雨、陨石、炸弹等）

--- @class FallingBodyComponent: table
--- @field speed number 下落速度
--- @field targetY number 目标Y坐标，物体下落的目标位置
--- @field isFalling boolean 是否正在下落
local M = {
    speed = 3.0,
    targetY = 0,
    isFalling = true
}

return M
