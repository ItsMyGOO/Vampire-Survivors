---
--- Created by echo.
--- DateTime: 2025/12/29 18:23
---
-- Battle.lua
local World                = require("ecs.world")
local EnemySpawn           = require("battle.enemy_spawn_system")
-- moving
local InputSys             = require("ecs.systems.player_input_system")
local VsSeekSys            = require("ecs.systems.vs_seek_system")
local MoveSys              = require("ecs.systems.movement_system")
-- weapon
local WeaponFireSystem     = require("ecs.systems.weapon_fire_system")
local ProjectileMoveSystem = require("ecs.systems.projectile_move_system")
local OrbitSystem          = require("ecs.systems.orbit_system")
-- anim
local AnimCmd              = require("ecs.systems.animation_command_system")
local AnimSys              = require("ecs.systems.animation_system")

---@class Battle
---@field world World?
local Battle               = {
    world = nil
}

local systems              = {
    InputSys,

    VsSeekSys,
    MoveSys,

    WeaponFireSystem,
    ProjectileMoveSystem,
    OrbitSystem,

    AnimCmd,
    AnimSys,
}

function Battle:StartBattle(stageCfg)
    local C = _G.ComponentRegistry

    local world = World.New()
    self.world = world
    world.grid = _G.Grid

    -- 生成玩家
    local player = world:CreateEntity()
    world.player_eid = player

    world:AddComponent(player, C.PlayerTag)
    world:AddComponent(player, C.Position)
    world:AddComponent(player, C.Velocity)
    world:AddComponent(player, C.MoveIntent, { speed = 2 })

    world:AddComponent(player, C.Animation, { clipSetId = "Player" })
    world:AddComponent(player, C.SpriteKey)
    world:AddComponent(player, C.AnimationCommand, { play_animation_name = "Run" })
    world:AddComponent(player, ComponentRegistry.WeaponSlots, {
        slots = {
            {
                def = "ProjectileKnife",
                timer = 0
            },
            {
                def = "OrbitKnife",
            },
        }
    })
    
    -- 生成敌人
    for i = 1, 10 do
        EnemySpawn.Spawn(world)
    end
end

function Battle:Tick(dt)
    local world = self.world

    if (world and world.grid) then
        world.grid:rebuild(world:GetComponentOfType(_G.ComponentRegistry.Position))
    end

    for _, system in ipairs(systems) do
        if system.update then
            -- 使用 xpcall 防止单个系统崩溃导致整个游戏退出
            local ok, err = xpcall(system.update, debug.traceback,
                system, world, dt)
            if not ok then
                print("❌ Lua Error in system.update:")
                print(err)
            end
        end
    end
end

function Battle.EndBattle()
    Battle.world = nil
    collectgarbage("collect") -- 只在切场景
end

return Battle
