---
--- Created by echo.
--- DateTime: 2025/12/23 22:22
---
--- 玩家输入系统
--- 处理玩家输入并更新玩家速度组件
local BaseSystem = require("ecs.base_system")

---@class PlayerInputSystem : BaseSystem
local PlayerInputSystem = {}
PlayerInputSystem.__index = PlayerInputSystem

function PlayerInputSystem.new(input)
    local self = BaseSystem.new()
    self.input = input -- 注入输入模块
    return setmetatable(self, PlayerInputSystem)
end

---@param world World
function PlayerInputSystem:start(world)
    BaseSystem.start(self, world)

    local PlayerTag = require("ecs.components.player_tag")
    local Velocity  = require("ecs.components.velocity")

    self.players    = world:GetComponentOfType(PlayerTag)
    self.velocities = world:GetComponentOfType(Velocity)
end

function PlayerInputSystem:update(dt)
    local ix = self.input:GetAxis("Horizontal")
    local iz = self.input:GetAxis("Vertical")

    for eid in pairs(self.players) do
        local vel = self.velocities[eid]
        if vel then
            vel.x = ix * vel.speed
            vel.z = iz * vel.speed
        end
    end
end

function PlayerInputSystem:shutdown()
    self.players = nil
    self.velocities = nil
end

return PlayerInputSystem
