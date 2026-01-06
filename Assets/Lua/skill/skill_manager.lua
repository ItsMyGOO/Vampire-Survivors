---
--- Created by echo.
--- DateTime: 2026/1/5 17:26
---
-- SkillManager.lua
SkillManager = {}
SkillManager.__index = SkillManager

function SkillManager.new(player)
    return setmetatable({
        player = player,
        skills = {},
    }, SkillManager)
end

function SkillManager:AddSkill(skill)
    table.insert(self.skills, skill)
end

function SkillManager:Update(dt)
    for _, skill in ipairs(self.skills) do
        skill:Update(dt, self.player)
    end
end
