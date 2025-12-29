--- Created by echo.
--- DateTime: 2025/12/28 20:40
---
-- AnimationDB.lua
---@class AnimationConfig
---@field sheet string 帧图片名称
---@field frames string[] 动画帧名称数组
---@field fps number 动画播放帧率
---@field loop boolean 是否循环播放

---@class PlayerAnimations
---@field Run AnimationConfig 跑动动画配置

---@class AnimationDB
---@field Player PlayerAnimations 玩家动画配置集合
AnimationDB = {
    Player = {
        Run = {
            sheet = "Farmer 0",
            frames = {
                "Run 0",
                "Run 1",
                "Run 2",
                "Run 3",
                "Run 4",
                "Run 5",
            },
            fps = 12,
            loop = true
        }
    }
}

return AnimationDB
