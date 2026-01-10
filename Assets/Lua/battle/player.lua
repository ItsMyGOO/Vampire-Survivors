---
--- Created by echo.
--- DateTime: 2026/1/5 18:01
---
-- Core/Player.lua
local PlayerStats = require("battle.player_stats")
local BuffManager = require("buff.buff_manager")

---@class Player
local Player = {}
Player.__index = Player

function Player.new()
    local self = setmetatable({}, Player)
    self.stats = PlayerStats.new()
    self.buffManager = BuffManager.new(self)
    self.expSystem = ExpSystem.new(self)
    return self
end

function Player:CollectProp(prop)
    if prop.exp then
        self.expSystem:AddExp(prop.exp)
    end

    -- 以后可以加：
    -- if prop.gold then ...
    -- if prop.buff then ...
end

return Player
