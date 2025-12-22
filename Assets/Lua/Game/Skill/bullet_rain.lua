---
--- Created by echo.
--- DateTime: 2025/12/22 15:19
---
-- skills/bullet_rain.lua
local Config = require("config.skills_config").bullet_rain
local BulletRainSystem = require("ecs.systems.bullet_rain_system")
local PrefabPool = require("utils.prefab_pool") -- 可选：对象池管理子弹预制体

local BulletRainSkill = {
    system = nil,
    isActive = false
}

function BulletRainSkill:Init(world, prefab)
    self.system = BulletRainSystem
    self.system.world = world
    self.system:Init(Config)
    self.system.prefab = prefab -- 传入子弹预制体
    world:AddSystem(self.system)
end

function BulletRainSkill:Cast()
    if self.isActive then return end
    print("💥 施放【子弹雨】技能！")
    self.system:Activate()
    self.isActive = true

    -- 3秒后自动结束（或由系统控制）
    -- 可扩展为 Lua 协程延时
    local co = coroutine.create(function()
        coroutine.wait(5.0) -- 自定义 wait 函数（基于 Time.deltaTime）
        self:End()
    end)
    table.insert(MainWorld.coroutines, co)
end

function BulletRainSkill:End()
    self.isActive = false
    self.system.isActive = false
end

return BulletRainSkill
