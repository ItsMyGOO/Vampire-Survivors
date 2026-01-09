---
--- Created by echo.
--- DateTime: 2026/1/9 15:13
---
EnemySpawnSystem = {
    timer = 0.0,
    interval = 1.0
}

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

    world:AddComponent(eid, ComponentRegistry.EnemyTag)

    world:AddComponent(eid, ComponentRegistry.Position, RandomPos(10))

    world:AddComponent(eid, ComponentRegistry.MoveIntent)
    world:AddComponent(eid, ComponentRegistry.Seek)

    world:AddComponent(eid, ComponentRegistry.SpriteKey)
    world:AddComponent(eid, ComponentRegistry.Animation, { clipSetId = "Enemy" })
    world:AddComponent(eid, ComponentRegistry.AnimationCommand, { play_animation_name = "Run" })

    world:AddComponent(eid, ComponentRegistry.Collider)
    world:AddComponent(eid, ComponentRegistry.Health)

    -- 返回给 C# 用来创建 Sprite
    return eid, enemyCfg and enemyCfg.spriteId
end

---@param world World
---@param dt number
function EnemySpawnSystem:update(world, dt)
    self.timer = self.timer + dt
    if self.timer < self.interval then
        return
    end
    self.timer = self.timer - self.interval

    -- 随时间变难
    local enemyCount = math.min(1 + math.floor(world.time / 10), 5)

    for i = 1, enemyCount do
        EnemySpawnSystem.Spawn(world)
    end
end

return EnemySpawnSystem
