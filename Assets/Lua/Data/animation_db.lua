--- Created by echo.
--- DateTime: 2025/12/28 20:40
---
-- AnimationDB.lua
---@class AnimationConfig
---@field name string 动画名称
---@field frame_count number 帧数量
---@field frames string[]? 动画帧名称数组
---@field fps number 动画播放帧率
---@field loop boolean 是否循环播放

---@class PlayerAnimations
---@field sheet string 帧图片名称
---@field Run AnimationConfig 跑动动画配置

---@class AnimationDB
---@field Player PlayerAnimations 玩家动画配置集合
AnimationDB = {
    Player = {
        sheet = "Assets/3rdParty/Undead Survivor/Sprites/Farmer 0.png",
        Run = {
            name = "Run",
            frame_count = 6,
            frames = nil,
            fps = 9,
            loop = true
        },
        Stand = {
            name = "Stand",
            frame_count = 4,
            frames = nil,
            fps = 6,
            loop = true
        }
    },
    Enemy = {
        sheet = "Assets/3rdParty/Undead Survivor/Sprites/Enemy 0.png",
        Run = {
            name = "Run",
            frame_count = 4,
            frames = nil,
            fps = 6,
            loop = true
        }
    }
}

return AnimationDB
