---
--- Created by echo.
--- DateTime: 2026/1/5 18:01
---
-- Core/Player.lua
local PlayerStats = require("battle.player_stats")
local BuffManager = require("buff.buff_manager")

local Player = {}
Player.__index = Player

function Player.new()
    local self = setmetatable({}, Player)
    self.stats = PlayerStats.new()
    self.buffManager = BuffManager.new(self)
    return self
end

return Player
