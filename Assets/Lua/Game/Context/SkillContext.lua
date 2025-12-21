---
--- Created by echo.
--- DateTime: 2025/12/21 11:00
---
local SkillContext = {}
SkillContext.__index = SkillContext

function SkillContext.new(params)
    return setmetatable({
        caster      = params.caster,
        target      = params.target,
        base_damage = params.base_damage or 0,
        damage      = 0,
        tags        = params.tags or {},
        buff_system = params.buff_system,
    }, SkillContext)
end

function SkillContext:has_tag(tag)
    for _, t in ipairs(self.tags) do
        if t == tag then return true end
    end
    return false
end

return SkillContext
