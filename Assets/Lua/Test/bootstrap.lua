---
--- Created by echo.
--- DateTime: 2025/12/21 10:59
---

-- XLua 会自动把 Lua 目录加入
-- 但你最好显式指定
package.path = package.path .. ";Assets/Lua/?.lua;Assets/Lua/?/init.lua"

print("=== lua skill pipeline demo start ===")

local fireball = require('Game.Skill.fireball')
local buff_system = require('Game.Buff.buff_system')
local fire_boost = require('Game.Buff.Impl.fire_boost')

local caster = { name = "hero", mp = 100 }
local enemy = { name = "slime", hp = 300 }

buff_system:add(fire_boost.new())

fireball.cast(caster, enemy)

print("enemy hp:", enemy.hp)
