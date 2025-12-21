---
--- Created by echo.
--- DateTime: 2025/12/21 11:00
---
local skill_config = require("Data.SkillConfig")

local SkillContext = {}
SkillContext.__index = SkillContext

function SkillContext.new(caster, target, skill_id)
    local cfg = skill_config[skill_id]

    return setmetatable({
        caster = caster,
        target = target,
        skill_id = skill_id,

        damage = cfg.base_damage,
        cost = cfg.cost,
        tags = cfg.tags,

        cancelled = false
    }, SkillContext)
end

function SkillContext:has_tag(tag)
    for _, t in ipairs(self.tags) do
        if t == tag then
            return true
        end
    end
    return false
end

return SkillContext
