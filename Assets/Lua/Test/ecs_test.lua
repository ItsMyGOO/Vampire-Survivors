---
--- Created by echo.
--- DateTime: 2025/12/22 21:42
---
print("🎮 Vampire Survivors Demo Start!")

require("Test.movement_system_test")()

-- 加载模块
local World = require("ecs.world")
local Transform = require("ecs.components.transform")
local Velocity = require("ecs.components.velocity")

-- 创建世界
MainWorld = World
MainWorld.eventBus = require("utils.event_bus")()

MainWorld:AddSystem(require("ecs.systems.movement_system")())
-- 全局暴露给 C#
_G.UpdateGame = function(dt)
    MainWorld:UpdateSystems(dt)
end

-- 创建玩家
function SpawnPlayer(x, z)
    local eid = MainWorld:AddEntity()

    local go = CS.UnityEngine.GameObject.Instantiate(CSPlayerPrefab)
    go.name = "Player"

    MainWorld:AddComponent(eid, Transform, {
        x = x or 0,
        y = 0,
        z = z or 0,
        go = go
    })

    MainWorld:AddComponent(eid, Velocity, {
        x = 0, y = 0, z = 0 -- 玩家暂时不动
    })

    CS.UnityEngine.Debug.Log("Player spawned at " .. (x or 0) .. ", " .. (z or 0))
    return eid
end

-- 创建敌人（自动向原点移动）
function SpawnEnemy(x, z)
    local eid = MainWorld:AddEntity()

    local go = CS.UnityEngine.GameObject.Instantiate(CSEnemyPrefab)
    go.name = "Enemy"

    MainWorld:AddComponent(eid, Transform, {
        x = x,
        y = 0,
        z = z,
        go = go
    })

    -- 计算朝向玩家（简化为朝向原点）
    local dirX, dirZ = -x, -z
    local len = math.sqrt(dirX * dirX + dirZ * dirZ)
    dirX, dirZ = dirX / len, dirZ / len

    MainWorld:AddComponent(eid, Velocity, {
        x = dirX * 2.0,
        y = 0,
        z = dirZ * 2.0,
        active = true
    })

    CS.UnityEngine.Debug.Log("Enemy spawned at " .. x .. ", " .. z)
    return eid
end

-- 暴露给 C#
_G.SpawnPlayer = SpawnPlayer
_G.SpawnEnemy = SpawnEnemy
