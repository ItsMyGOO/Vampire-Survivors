---
--- Created by echo.
--- DateTime: 2026/1/5 19:33
---
-- Core/Buff/BuffExecutor.lua
local BuffExecutor = {
    EffectHandlers = {}
}

function BuffExecutor.Register(type, fn)
    BuffExecutor.EffectHandlers[type] = fn
end

function BuffExecutor.Execute(effectList, player, ctx, stack)
    if not effectList then return end
    for _, effect in ipairs(effectList) do
        local handler = BuffExecutor.EffectHandlers[effect.type]
        if not handler then
            error("No Effect Handler: " .. effect.type)
        end
        handler(player, effect, ctx, stack or 1)
    end
end

return BuffExecutor
