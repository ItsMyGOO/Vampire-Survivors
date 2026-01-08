---
--- Created by echo.
--- DateTime: 2026/1/8 17:32
---
-- components/DamageSource.lua
return {
    owner         = -1,           -- 谁造成的伤害（player eid）
    damage        = 0,            -- 伤害值

    knockbackMode = "fromSource", -- | "fromOwner" | "custom"
    knockback     = 0,            -- 击退强度

    mode          = "single",     -- "pierce" ,"persistent",
    tickInterval  = 0.3,          -- persistent 专用
    hitTargets    = nil           -- [eid] = timer
}
