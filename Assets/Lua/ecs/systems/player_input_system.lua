---
--- Created by echo.
--- DateTime: 2025/12/23 22:22
---
--- 玩家输入系统
--- 处理玩家输入并更新玩家速度组件
---@class PlayerInputSystem
local PlayerInputSystem = {
}
PlayerInputSystem.__index = PlayerInputSystem

---@param world World
---@param dt number
function PlayerInputSystem:update(world, dt)
    local input = CS.UnityEngine.Input
    local hori = input.GetAxisRaw("Horizontal")
    local vert = input.GetAxisRaw("Vertical")

    local player = world.player_eid
    ---@type table<integer, VelocityComponent>
    local velocities = world:GetComponentOfType(_G.ComponentRegistry.Velocity)

    local vel = velocities[player]

    vel.x = hori * vel.speed
    vel.y = vert * vel.speed
end

return PlayerInputSystem
