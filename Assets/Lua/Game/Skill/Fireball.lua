---
--- Created by echo.
--- DateTime: 2025/12/21 11:13
---
local SkillPipeline = require("Game.Pipeline.SkillPipeline")
local SkillContext  = require("Game.Context.SkillContext")

local PreCast       = require("Game.Pipeline.Stage.PreCast")
local DamageCalc    = require("Game.Pipeline.Stage.DamageCalc")
local DamageApply   = require("Game.Pipeline.Stage.DamageApply")

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
