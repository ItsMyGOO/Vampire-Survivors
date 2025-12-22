---
--- Created by echo.
--- DateTime: 2025/12/21 21:46
---
---
--- 处理实体移动的系统
--- @class MovementSystem
--- @field world World? ECS世界实例
--- @field transforms table? Transform组件池
--- @field velocities table? Velocity组件池
local MovementSystem = {
    world = nil,
    transforms = nil,
    velocities = nil
}

---
--- 系统启动时调用，初始化系统所需组件
--- @param world World ECS世界实例
function MovementSystem:start(world)
    self.world = world

    -- 使用 schema 获取真实名称
    local Transform = require("ecs.components.transform")
    local Velocity = require("ecs.components.velocity")

    self.transforms = self.world:GetComponentOfType(Transform)
    self.velocities = self.world:GetComponentOfType(Velocity)
end

---
--- 更新系统逻辑，处理实体移动
--- @param dt number 帧间隔时间
function MovementSystem:update(dt)
    if not self.transforms or not self.velocities then return end

    local transforms = self.transforms
    local velocities = self.velocities

    for eid, vel in pairs(velocities) do
        local trans = transforms[eid]
        if trans and vel.active ~= false then
            trans.x = (trans.x or 0) + (vel.x or 0) * dt
            trans.y = (trans.y or 0) + (vel.y or 0) * dt
            trans.z = (trans.z or 0) + (vel.z or 0) * dt

            -- 同步到 Unity（可选）
            if trans.go then
                trans.go.transform.position = CS.UnityEngine.Vector3(trans.x, trans.y, trans.z)
            end
        end
    end
end

return MovementSystem
