---
--- Created by echo.
--- DateTime: 2025/12/29 18:24
---
-- EnemySpawnSystem.lua
local ComponentRegistry = require("ecs.component_registry")

local EnemySpawnSystem = {}

math.randomseed(os.time())

---@param range integer
local function RandomPos(range)
    return {
        x = math.random(-range, range),
        y = math.random(-range, range),
        z = 0
    }
end

function EnemySpawnSystem.Spawn(world, enemyCfg)
    local eid = world:CreateEntity()

    world:AddComponent(eid, ComponentRegistry.Position, RandomPos(2))
    world:AddComponent(eid, ComponentRegistry.Velocity)
    world:AddComponent(eid, ComponentRegistry.Chase)

    world:AddComponent(eid, ComponentRegistry.Animation, {
        clipSetId = "Enemy"
    })
    world:AddComponent(eid, ComponentRegistry.SpriteKey)
    world:AddComponent(eid, ComponentRegistry.AnimationCommand, {
        play_animation_name = "Run"
    })

    -- 返回给 C# 用来创建 Sprite
    return eid, enemyCfg and enemyCfg.spriteId
end

return EnemySpawnSystem
