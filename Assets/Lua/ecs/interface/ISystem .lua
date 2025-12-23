---
--- Created by echo.
--- DateTime: 2025/12/22 16:05
---
--- ECS系统接口定义
---@interface ISystem
---@field world World
local ISystem = {}

---
--- 系统启动时调用
--- @param world World
function ISystem:start(world) end

---
--- 系统更新逻辑
--- @param dt number 帧间隔时间
function ISystem:update(dt) end

---
--- 系统关闭时调用
function ISystem:shutdown() end

return ISystem
