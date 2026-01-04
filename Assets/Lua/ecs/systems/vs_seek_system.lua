---
--- Created by echo.
--- DateTime: 2026/1/1 20:40
---
---
--- Vampire Survivors style Seek System
--- Created by echo (refined)
---

---@class VSSeekSystem
local VSSeekSystem = {}
VSSeekSystem.__index = VSSeekSystem

-- =========================
-- 参数区（核心调参点）
-- =========================

local LOCK_RADIUS = 1.2       -- 最内圈（贴近玩家）
local MID_RADIUS  = 4.0       -- 减速区
local FAR_RADIUS  = 10.0      -- 远距离

local MIN_SPEED_FACTOR = 0.15 -- ❗永不为 0（关键）
local GLOBAL_LAG_SPEED = 5.0  -- 玩家目标滞后
local LAG_SCALE        = 10.0

-- 近远敌人响应差异
local NEAR_LAG = 0.25
local FAR_LAG  = 1.0

-- =========================
-- 工具函数
-- =========================

local function Clamp01(v)
    if v < 0 then return 0 end
    if v > 1 then return 1 end
    return v
end

-- 环偏移（你已有就直接用你的）
-- 这里只是示例
local function GetRingOffset(eid)
    local angle = (eid * 0.618) % 1 * math.pi * 2
    local r = 0.8
    return {
        x = math.cos(angle) * r,
        y = math.sin(angle) * r
    }
end

-- =========================
-- 主逻辑
-- =========================

function VSSeekSystem:update(world, dt)
    local C = _G.ComponentRegistry

    local positions = world:GetComponentOfType(C.Position)
    local intents   = world:GetComponentOfType(C.MoveIntent)
    local seeks     = world:GetComponentOfType(C.Seek)

    local playerPos = positions[world.player_eid]
    if not playerPos then return end

    -- ==================================================
    -- 1️⃣ 全局目标点滞后（所有敌人共享）
    -- ==================================================
    world.seekTarget = world.seekTarget or {
        x = playerPos.x,
        y = playerPos.y
    }

    local globalLag = 1 - math.exp(-GLOBAL_LAG_SPEED * dt)
    world.seekTarget.x =
        world.seekTarget.x + (playerPos.x - world.seekTarget.x) * globalLag
    world.seekTarget.y =
        world.seekTarget.y + (playerPos.y - world.seekTarget.y) * globalLag

    -- ==================================================
    -- 2️⃣ 每个敌人的 Seek
    -- ==================================================
    for eid, intent in pairs(intents) do
        local pos  = positions[eid]
        local seek = seeks[eid]
        if not pos or not seek then goto continue end

        -- 到玩家的原始距离
        local dx0 = world.seekTarget.x - pos.x
        local dy0 = world.seekTarget.y - pos.y
        local dist0 = math.sqrt(dx0 * dx0 + dy0 * dy0)

        -- ==================================================
        -- 径向滞后（远的反应慢，近的更灵敏）
        -- ==================================================
        local tLag = Clamp01(dist0 / FAR_RADIUS)
        local lag  = NEAR_LAG + (FAR_LAG - NEAR_LAG) * tLag
        lag = 1 - math.exp(-lag * LAG_SCALE * dt)

        -- ==================================================
        -- 个体目标点（玩家目标 + 环偏移）
        -- ==================================================
        local off = GetRingOffset(eid)

        -- 环偏移随距离衰减（避免硬环）
        local ringScale = Clamp01(dist0 / FAR_RADIUS)
        off.x = off.x * ringScale
        off.y = off.y * ringScale

        local tx = pos.x + (world.seekTarget.x + off.x - pos.x) * lag
        local ty = pos.y + (world.seekTarget.y + off.y - pos.y) * lag

        local dx = tx - pos.x
        local dy = ty - pos.y
        local dist = math.sqrt(dx * dx + dy * dy)

        if dist < 1e-5 then
            intent.dirX = 0
            intent.dirY = 0
            intent.speed = 0
            goto continue
        end

        -- ==================================================
        -- 朝向（永远指向目标点）
        -- ==================================================
        intent.dirX = dx / dist
        intent.dirY = dy / dist

        -- ==================================================
        -- 3️⃣ 距离 → 速度（VS 核心）
        -- ==================================================
        local speed
        local baseSpeed = seek.speed or 1.0

        if dist <= LOCK_RADIUS then
            speed = baseSpeed * MIN_SPEED_FACTOR
        elseif dist <= MID_RADIUS then
            local t = (dist - LOCK_RADIUS) / (MID_RADIUS - LOCK_RADIUS)
            t = Clamp01(t)
            speed = baseSpeed * (MIN_SPEED_FACTOR + (1 - MIN_SPEED_FACTOR) * t * t)
        elseif dist <= FAR_RADIUS then
            local t = (dist - MID_RADIUS) / (FAR_RADIUS - MID_RADIUS)
            t = Clamp01(t)
            speed = baseSpeed * (0.4 + 0.6 * t)
        else
            speed = baseSpeed
        end

        intent.speed = speed

        ::continue::
    end
end

return VSSeekSystem
