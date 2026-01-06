---
--- Created by echo.
--- DateTime: 2026/1/5 17:03
---
-- Core/PlayerStats.lua
local PlayerStats = {}
PlayerStats.__index = PlayerStats

function PlayerStats.new()
    return setmetatable({
        damage = 10,
        projectileCount = 1,
    }, PlayerStats)
end

function PlayerStats:Modify(stat, value)
    self[stat] = self[stat] + value
    print("[Stats]", stat, "=", self[stat])
end

return PlayerStats
