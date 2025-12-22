---
--- Created by echo.
--- DateTime: 2025/12/22 16:28
---
-- components/falling_body.lua
-- 表示任何会下落的物体（子弹雨、陨石、炸弹等）
local M = {
    speed = 3.0,
    targetY = 0,
    isFalling = true
}

return setmetatable(M, { __name = "FallingBody" })
