---
--- Created by echo.
--- DateTime: 2025/12/22 16:05
---
--- ECS系统接口定义
---@interface ISystem
---@field world World ECS世界实例
local ISystem = {}

---
--- 系统启动时调用
function ISystem:start() end

---
--- 系统更新逻辑
--- @param dt number 帧间隔时间
function ISystem:update(dt) end

---
--- 系统关闭时调用
function ISystem:shutdown() end

return ISystem
