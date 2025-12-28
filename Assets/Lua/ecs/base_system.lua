---
--- Created by echo.
--- DateTime: 2025/12/23 21:25
---
-- ECS系统基类定义
-- 定义所有ECS系统的通用接口和生命周期管理
---@class BaseSystem
local BaseSystem = {
    ---@type World?
    world = nil
}
BaseSystem.__index = BaseSystem

--- 创建新的系统实例
---@return table
function BaseSystem.new()
    local self = setmetatable({}, BaseSystem)
    return self
end

--- 系统启动方法
--- @param world World ECS世界实例
function BaseSystem:start(world)
    self.world = world
end

--- 系统更新方法
--- @param dt number 帧时间间隔
function BaseSystem:update(dt)
    -- 子类实现
end

--- 系统关闭方法
function BaseSystem:shutdown()
end

return BaseSystem
