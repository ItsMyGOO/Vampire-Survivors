---
--- Created by echo.
--- DateTime: 2025/12/25 08:25
---

--- @class ChaseComponent: table
--- @field speed number 追逐速度
--- @field stop_distance number 停止距离，当与目标距离小于此值时停止移动
--- @field target_eid integer 目标实体ID
local M = {
    speed = 0.5,
    stop_distance = 0.5,
    target_eid = -1
}

return M
