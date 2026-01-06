---
--- Created by echo.
--- DateTime: 2026/1/5 21:44
---
-- Main.lua

-- 先加载所有 Effect（非常重要）
require("buff.buff_effects")

local Player = require("battle.player")
local BuffDefs = require("Data.buff_defs")
local Trigger = require("buff.buff_trigger")

local player = Player.new()

-- 模拟三选一
player.buffManager:AddBuff(BuffDefs.DamageUp)
player.buffManager:AddBuff(BuffDefs.DamageUp)
player.buffManager:AddBuff(BuffDefs.CritExplosion)

print("\n=== Hit ===")

player.buffManager:Trigger(Trigger.OnHit, {
    targetPos = "(10,5)",
    isCrit = true
})
