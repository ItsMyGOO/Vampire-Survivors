---
--- Created by echo.
--- DateTime: 2025/12/20 17:29
---

local M = {}

function M.Cast(ctx)
    print("Fireball cast")

    ctx.Damage.FinalDamage = ctx.Damage.BaseDamage

    -- 这里可以加 Lua 内部逻辑
    if math.random() < 0.3 then
        ctx.Damage.FinalDamage = ctx.Damage.FinalDamage * 1.5
    end
end

return M
