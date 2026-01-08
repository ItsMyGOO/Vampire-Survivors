---
--- Created by echo.
--- DateTime: 2026/1/8 17:32
---
-- components/DamageSource.lua
return {
    owner   = -1,      -- 谁造成的伤害（player eid）
    damage  = 0,       -- 伤害值
    knockback = 0,     -- 击退强度
    hitOnce = true,    -- 是否命中一次就失效
}
