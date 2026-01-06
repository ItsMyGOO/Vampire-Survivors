---
--- Created by echo.
--- DateTime: 2026/1/5 17:31
---
-- ExpSystem.lua
ExpSystem = {}
ExpSystem.__index = ExpSystem

function ExpSystem.new(player)
    return setmetatable({
        player = player,
        level = 1,
        exp = 0,
        expToNext = 10,
    }, ExpSystem)
end

function ExpSystem:AddExp(value)
    self.exp = self.exp + value * self.player.stats.expGain
    if self.exp >= self.expToNext then
        self:LevelUp()
    end
end

function ExpSystem:LevelUp()
    self.level = self.level + 1
    self.exp = self.exp - self.expToNext
    self.expToNext = self.expToNext * 1.3
    self.player.buffManager:Trigger(BuffTrigger.OnLevelUp)
end
