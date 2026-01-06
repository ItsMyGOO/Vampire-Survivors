---
--- Created by echo.
--- DateTime: 2026/1/5 17:51
---
-- BuffPool.lua
local BuffDefs = require("Data.buff_defs")

local BuffPool = {
    BuffDefs.DamageUp,
    BuffDefs.KnifeExplosion,
}

function RollBuffs(n)
    local result = {}
    for i = 1, n do
        table.insert(result, BuffPool[math.random(#BuffPool)])
    end
    return result
end
