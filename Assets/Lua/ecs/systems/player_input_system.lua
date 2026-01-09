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
    local input = _G.InputData

    local player = world.player_eid
    ---@type table<integer, MoveIntentComponent>
    local intents = world:GetComponentOfType(_G.ComponentRegistry.MoveIntent)

    local intent = intents[player]

    intent.dirX = input.hori 
    intent.dirY = input.vert 
end

return PlayerInputSystem
