---
--- Created by echo.
--- DateTime: 2025/12/21 11:13
---
local SkillContext = require("Game.Context.SkillContext")
local SkillPipeline = require("Game.Pipeline.SkillPipeline")

local pre_cast = require("Game.Pipeline.Stage.PreCast")
local damage_calc = require("Game.Pipeline.Stage.DamageCalc")
local damage_apply = require("Game.Pipeline.Stage.DamageApply")

local Fireball = {}

function Fireball.cast(caster, target)
    local ctx = SkillContext.new(caster, target, "fireball")

    local pipeline = SkillPipeline.new({
        pre_cast,
        damage_calc,
        damage_apply
    })

    pipeline:execute(ctx)
end

return Fireball
