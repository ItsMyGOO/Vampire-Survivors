---
--- Created by echo.
--- DateTime: 2026/1/7 17:56
---
-- main.lua
local World                = require("ecs.world")

-- components
local ComponentRegistry    = require("ecs.component_registry")

-- systems
local WeaponFireSystem     = require("ecs.systems.weapon_fire_system")
local ProjectileMoveSystem = require("ecs.systems.projectile_move_system")
local OrbitSystem          = require("ecs.systems.orbit_rotate_system")

local systems              = {
    WeaponFireSystem,
    ProjectileMoveSystem,
    OrbitSystem
}

-- ---------- 创建 World ----------
local world                = World.New()

-- ---------- 创建玩家 ----------
local player               = world:CreateEntity()
world:AddComponent(player, ComponentRegistry.Position, { x = 0, y = 0 })
world:AddComponent(player, ComponentRegistry.PlayerTag)
world:AddComponent(player, ComponentRegistry.WeaponSlot, {
    def = "ProjectileKnife",
    timer = 0
})

-- 再给一个 Orbit 武器
world:AddComponent(player, ComponentRegistry.WeaponSlot, {
    def = "OrbitKnife",
    timer = 0
})

print("Player EID =", player)

-- ---------- 创建敌人 ----------
local function spawnEnemy(x, y)
    local e = world:CreateEntity()
    world:AddComponent(e, ComponentRegistry.Position, { x = x, y = y })
    world:AddComponent(e, ComponentRegistry.EnemyTag)
    print("Enemy EID =", e, "pos =", x, y)
end

spawnEnemy(5, 0)
spawnEnemy(3, 4)

-- ---------- 简单循环 ----------
local time = 0
local dt = 0.2

for frame = 1, 20 do
    time = time + dt
    print("\n--- Frame", frame, "Time", time, "---")

    for _, sys in ipairs(systems) do
        sys:update(world, dt)
    end

    -- 打印所有 Projectile
    local proj  = world:GetComponentOfType(ComponentRegistry.Projectile)
    local pos   = world:GetComponentOfType(ComponentRegistry.Position)
    local vel   = world:GetComponentOfType(ComponentRegistry.Velocity)
    local orbit = world:GetComponentOfType(ComponentRegistry.Orbit)

    for eid, p in pairs(proj) do
        local position = pos[eid]
        if orbit[eid] then
            print(string.format(
                "[Orbit] eid=%d  pos=(%.2f, %.2f)",
                eid, position.x, position.y
            ))
        elseif vel[eid] then
            print(string.format(
                "[Projectile] eid=%d  pos=(%.2f, %.2f) vel=(%.2f, %.2f)",
                eid, position.x, position.y, vel[eid].vx, vel[eid].vy
            ))
        end
    end
end
