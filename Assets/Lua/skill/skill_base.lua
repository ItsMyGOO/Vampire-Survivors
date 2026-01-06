---
--- Created by echo.
--- DateTime: 2026/1/5 17:23
---
-- SkillBase.lua
SkillBase = {}
SkillBase.__index = SkillBase

function SkillBase.new(interval)
    return setmetatable({
        interval = interval,
        timer = 0,
    }, SkillBase)
end

function SkillBase:Update(dt, player)
    self.timer = self.timer + dt
    if self.timer >= self.interval then
        self.timer = self.timer - self.interval
        self:Fire(player)
    end
end

function SkillBase:Fire(player) end
