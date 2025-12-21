---
--- Created by echo.
--- DateTime: 2025/12/21 11:13
---
local SkillPipeline = require("Game.Pipeline.skill_pipeline")
local SkillContext  = require('Game.Context.skill_context')

local PreCast       = require("Game.Pipeline.Stage.pre_cast")
local DamageCalc    = require("Game.Pipeline.Stage.damage_calc")
local DamageApply   = require("Game.Pipeline.Stage.damage_apply")

local Fireball      = {}

function Fireball.cast(params)
    local ctx = SkillContext.new(params)

    local pipeline = SkillPipeline.new({
        PreCast,
        DamageCalc,
        DamageApply
    })

    pipeline:execute(ctx)
end

return Fireball
