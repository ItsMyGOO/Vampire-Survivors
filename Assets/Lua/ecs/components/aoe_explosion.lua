---
--- Created by echo.
--- DateTime: 2025/12/22 15:21
---
-- components/aoe_explosion.lua

--- @class AoeExplosionComponent: table
--- @field radius number 爆炸半径
--- @field damage number 伤害值
--- @field hasExploded boolean 是否已经爆炸
local M = {
    radius = 2.5,
    damage = 50,
    hasExploded = false
}

return M
