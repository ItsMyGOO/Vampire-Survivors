---
--- Created by echo.
--- DateTime: 2025/12/22 15:30
---
-- 控制子弹雨的生成与下落
local BulletRainDrop = require("ecs.components.bullet_rain_drop")
local Transform = require("ecs.components.transform")
local LifeTime = require("ecs.components.life_time")

---
--- 控制子弹雨的生成与下落
--- @class BulletRainSystem
--- @field world World ECS世界实例
--- @field nextSpawnTime number 下一次生成子弹的时间戳
--- @field dropsRemaining integer 剩余待生成的子弹数量
--- @field isActive boolean 是否处于激活状态
--- @field config table 子弹雨配置参数表
--- @field durationEnd number 持续时间结束时间戳
--- @field prefab table 子弹预制体
local BulletRainSystem = {
    world = nil,
    nextSpawnTime = 0,
    dropsRemaining = 0,
    isActive = false,
    config = {},
}

---
--- 初始化系统
--- @param config table 配置参数表
function BulletRainSystem:Init(config)
    self.config = config
    self.isActive = false
end

---
--- 激活系统并设置持续时间
--- @param duration number? 可选的持续时间（秒）
function BulletRainSystem:Activate(duration)
    self.isActive = true
    self.dropsRemaining = self.config.total_drops
    self.nextSpawnTime = os.clock()
    self.durationEnd = os.clock() + (duration or self.config.duration)
end

---
--- 更新系统逻辑
--- @param dt number 帧间隔时间
function BulletRainSystem:Update(dt)
    if not self.isActive then return end
    local now = os.clock()

    -- 结束判断
    if now >= self.durationEnd then
        self.isActive = false
        return
    end

    -- 生成新子弹
    if self.dropsRemaining > 0 and now >= self.nextSpawnTime then
        self:SpawnDrop()
        self.nextSpawnTime = now + self.config.interval
        self.dropsRemaining = self.dropsRemaining - 1
    end
end

---
--- 生成一个子弹雨滴
function BulletRainSystem:SpawnDrop()
    local area = self.config.spawn_area
    local x = math.random() * area.x - area.x / 2
    local z = math.random() * area.x - area.z / 2
    local y = self.config.min_height

    local eid = self.world:AddEntity()

    -- 添加变换（绑定 GameObject）
    local go = CS.UnityEngine.GameObject.Instantiate(self.prefab, CS.UnityEngine.Vector3(x, y, z),
        CS.UnityEngine.Quaternion.identity)
    self.world:AddComponent(eid, Transform, {
        x = x,
        y = y,
        z = z,
        go = go
    })

    -- 添加下落组件
    self.world:AddComponent(eid, BulletRainDrop, {
        speed = self.config.fall_speed,
        targetY = 0,
    })

    -- 添加生命周期（自动销毁）
    self.world:AddComponent(eid, LifeTime, {
        remaining = 3.0, -- 爆炸后最多存在3秒
        onExpire = function()
            if go then CS.UnityEngine.Object.Destroy(go) end
            self.world:RemoveEntity(eid)
        end
    })
end

return BulletRainSystem
