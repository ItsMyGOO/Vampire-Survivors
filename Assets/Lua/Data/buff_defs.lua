---
--- Created by echo.
--- DateTime: 2026/1/5 19:32
---
-- Data/BuffDefs.lua
return {

    DamageUp = {
        id = "DamageUp",
        maxStack = 3,

        onAdd = {
            { type = "StatAdd", stat = "damage", value = 2 }
        }
    },

    CritExplosion = {
        id = "CritExplosion",
        maxStack = 1,

        onTrigger = {
            OnHit = { -- ← 纯字符串
                {
                    type = "If",
                    cond = "isCrit",
                    effects = {
                        { type = "Explosion", radius = 3.0, damage = 20 }
                    }
                }
            }
        }
    }

}
