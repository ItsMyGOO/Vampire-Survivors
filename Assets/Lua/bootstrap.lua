---
--- Created by echo.
--- DateTime: 2025/12/21 12:18
---
print("lua bootstrap start")

FireBall = require('Game.Skill.fireball')
BuffSystem = require('Game.Buff.buff_system')
FireBoost = require('Game.Buff.Impl.fire_boost')

local World = require("ecs.world")
local Transform = require("ecs.components.transform")
local Velocity = require("ecs.components.velocity")
local Health = require("ecs.components.health")

local MovementSystem = require("ecs.systems.movement_system")

-- 创建世界
MainWorld = World

-- 注册系统
MainWorld:AddSystem(MovementSystem)
--MainWorld:AddSystem(require("ecs.systems.combat_system"))

-- 创建玩家实体
local playerEid = MainWorld:AddEntity()
MainWorld:AddComponent(playerEid, Transform, {
    x = 0,
    y = 0,
    z = 0,
    go = nil -- 稍后绑定
})
MainWorld:AddComponent(playerEid, Health, { value = 100 })

-- 全局暴露给 C#
_G.MainWorld = MainWorld
_G.SpawnEnemy = function(go, x, y, z)
    local eid = MainWorld:AddEntity()
    MainWorld:AddComponent(eid, Transform, { x = x, y = y, z = z, go = go })
    MainWorld:AddComponent(eid, Velocity, { x = 0, y = 0, z = -1, active = true })
    MainWorld:AddComponent(eid, Health, { value = 10 })
    return eid
end
