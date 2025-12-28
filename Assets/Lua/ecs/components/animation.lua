---
--- Created by echo.
--- DateTime: 2025/12/28 19:37
---

--- @class AnimationComponent
--- @field clipSetId  string? 动画剪辑列表
--- @field clipId string? 动画剪辑ID，可能为nil表示无动画
--- @field frame integer 当前帧数
--- @field time number 动画播放时间
--- @field playing boolean 是否正在播放动画
local M = {
    clipSetId = nil,  -- "Player" / "EnemyA"
    clipId    = nil,

    frame     = 1,
    time      = 0,
    playing   = false,
}

return M
