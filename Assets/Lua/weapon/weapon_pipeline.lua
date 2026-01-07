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
        if step then
            step(ctx)
        else
            print("[WeaponPipeline] step not found: " .. stepName)
        end
    end
end

return Pipeline
