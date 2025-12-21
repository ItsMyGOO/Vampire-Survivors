---
--- Created by echo.
--- DateTime: 2025/12/21 11:03
---
local SkillPipeline = {}
SkillPipeline.__index = SkillPipeline

function SkillPipeline.new(stages)
    return setmetatable({
        stages = stages
    }, SkillPipeline)
end

function SkillPipeline:execute(ctx)
    for _, stage in ipairs(self.stages) do
        stage:execute(ctx) -- 必须用 :
    end
end

return SkillPipeline
