---
--- Created by echo.
--- DateTime: 2026/1/7 17:43
---
-- weapon/WeaponPipeline.lua
local Steps = require("weapon.weapon_steps")

local Pipeline = {}

function Pipeline.Execute(ctx)
    for _, stepName in ipairs(ctx.weaponDef.pipeline) do
        local step = Steps[stepName]
         step(ctx)
    end
end

return Pipeline
