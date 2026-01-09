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
        Idle = {
            name = "Stand",
            frame_count = 4,
            frames = nil,
            fps = 6,
            loop = true
        }
    },
    Zombie1 = {
        sheet = "Assets/3rdParty/Undead Survivor/Sprites/Enemy 0.png",
        Run = {
            name = "Run",
            frame_count = 4,
            frames = nil,
            fps = 6,
            loop = true
        },
        Hit = {
            name = "Hit",
            frame_count = 1,
            frames = nil,
            fps = 2,
            loop = false
        }
    },
    Zombie2 = {
        sheet = "Assets/3rdParty/Undead Survivor/Sprites/Enemy 1.png",
        Run = {
            name = "Run",
            frame_count = 6,
            frames = nil,
            fps = 9,
            loop = true
        },
        Hit = {
            name = "Hit",
            frame_count = 1,
            frames = nil,
            fps = 2,
            loop = false
        }
    },
    Skeleton1 = {
        sheet = "Assets/3rdParty/Undead Survivor/Sprites/Enemy 2.png",
        Run = {
            name = "Run",
            frame_count = 6,
            frames = nil,
            fps = 9,
            loop = true
        },
        Hit = {
            name = "Hit",
            frame_count = 1,
            frames = nil,
            fps = 2,
            loop = false
        }
    },
    Skeleton2 = {
        sheet = "Assets/3rdParty/Undead Survivor/Sprites/Enemy 3.png",
        Run = {
            name = "Run",
            frame_count = 6,
            frames = nil,
            fps = 9,
            loop = true
        },
        Hit = {
            name = "Hit",
            frame_count = 1,
            frames = nil,
            fps = 2,
            loop = false
        }
    },
}

return AnimationDB
