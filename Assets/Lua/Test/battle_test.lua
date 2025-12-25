---
--- Created by echo.
--- DateTime: 2025/12/23 22:39
---
-- 加载模块
local World = require("ecs.world")

local Transform = require("ecs.components.transform")
local Velocity = require("ecs.components.velocity")

-- 创建世界
MainWorld = World
MainWorld.eventBus = require("utils.event_bus")()

MainWorld:AddSystem(require("ecs.systems.player_input_system").new(CS.UnityEngine.Input))
MainWorld:AddSystem(require("ecs.systems.enemy_ai_system").new())
MainWorld:AddSystem(require("ecs.systems.movement_system").new())
MainWorld:AddSystem(require("ecs.systems.transform_sync_system").new())

-- 全局暴露给 C#
_G.UpdateGame = function(dt)
    MainWorld:UpdateSystems(dt)
end

-- 创建玩家
function SpawnPlayer(x, z)
    local eid = MainWorld:AddEntity()

    local go = CS.UnityEngine.GameObject.Instantiate(CSPlayerPrefab)
    go.name = "Player"

    MainWorld:AddComponent(eid, require("ecs.components.player_tag"))
    MainWorld:AddComponent(eid, Transform, { go = go })
    MainWorld:AddComponent(eid, Velocity)

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
