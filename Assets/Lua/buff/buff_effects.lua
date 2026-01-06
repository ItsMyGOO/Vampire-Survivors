---
--- Created by echo.
--- DateTime: 2026/1/5 19:33
---
local BuffExecutor = require("buff.buff_executor")

BuffExecutor.Register("StatAdd", function(player, effect, ctx, stack)
    player.stats:Modify(effect.stat, effect.value * stack)
end)
-- Effect_Explosion.lua
BuffExecutor.Register("Explosion", function(player, effect, ctx, stack)
    print(string.format(
        "[Explosion] pos=%s radius=%.1f damage=%d",
        tostring(ctx.targetPos),
        effect.radius,
        effect.damage * stack
    ))
end)
-- Effect_If.lua
BuffExecutor.Register("If", function(player, effect, ctx, stack)
    if not ctx then return end
    if ctx[effect.cond] then
        BuffExecutor.Execute(effect.effects, player, ctx, stack)
    end
end)
