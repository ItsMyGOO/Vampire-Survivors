---
--- Created by echo.
--- DateTime: 2025/12/22 16:28
---
-- components/falling_body.lua
-- 表示任何会下落的物体（子弹雨、陨石、炸弹等）
return {
    ---@type number 下落速度
    speed = 3.0,
    ---@type number 目标高度（如地面）
    targetY = 0,
    ---@type boolean 是否正在下落
    isFalling = true
}