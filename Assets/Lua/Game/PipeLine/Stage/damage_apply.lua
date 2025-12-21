---
--- Created by echo.
--- DateTime: 2025/12/21 11:05
---
local DamageApply = {}

function DamageApply:execute(ctx)
     CS.LuaSkillAPI.ApplyDamage(ctx.target.id, ctx.damage)

    print(string.format(
        "%s deals %d damage to %s",
        ctx.caster.name,
        ctx.damage,
        ctx.target.name
    ))
end

return DamageApply
