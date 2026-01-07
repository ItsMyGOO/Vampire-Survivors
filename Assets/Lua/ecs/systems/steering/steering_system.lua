---
--- Created by echo.
--- DateTime: 2025/12/31 16:01
---
---
--- Created by echo.
--- Refactored by ChatGPT.
--- DateTime: 2025/12/31
---

---@class SteeringSystem
local SteeringSystem = {}
SteeringSystem.__index = SteeringSystem

----------------------------------------------------------------
-- Seek：只负责「逻辑意图 / 朝向」
----------------------------------------------------------------
---@param seek SeekComponent
---@param pos PositionComponent
---@param target PositionComponent
---@param intent MoveIntentComponent
local function ApplySeek(seek, pos, target, intent)
    local dx = target.x - pos.x
    local dy = target.y - pos.y
    local distSq = dx * dx + dy * dy

    if distSq < seek.stop_distance * seek.stop_distance then
        intent.dirX = 0
        intent.dirY = 0
        intent.speed = 0
        return
    end

    local dist = math.sqrt(distSq)
    intent.dirX = dx / dist
    intent.dirY = dy / dist
    intent.speed = seek.speed
end

----------------------------------------------------------------
-- Neighbor 收集 + 规则过滤（集中处理）
----------------------------------------------------------------
---@param world World
---@param pos PositionComponent
---@param selfId integer
---@param radius number
---@return table -- GridNeighbor[]
local function CollectSeparationNeighbors(world, pos, selfId, radius)
    local grid = world.grid
    if not grid then return {} end

    local raw = grid:query(pos.x, pos.y, radius)
    local neighbors = {}

    for _, n in ipairs(raw) do
        -- 规则集中在这里
        if n.eid ~= selfId and n.eid ~= world.player_eid then
            neighbors[#neighbors + 1] = n
        end
    end

    return neighbors
end

----------------------------------------------------------------
-- Separation：纯数学，不知道 world / grid / player
----------------------------------------------------------------
---@class SeparationContext
---@field pos PositionComponent
---@field intent MoveIntentComponent
---@field radius number
---@field weight number
---@field neighbors table

---@param ctx SeparationContext
---@return number, number -- sepFx, sepFy
local function ComputeSeparation(ctx)
    local fx, fy = 0.0, 0.0
    local px, py = ctx.pos.x, ctx.pos.y
    local r2 = ctx.radius * ctx.radius

    for _, n in ipairs(ctx.neighbors) do
        local dx = px - n.x
        local dy = py - n.y
        local distSq = dx * dx + dy * dy

        if distSq > 0 and distSq < r2 then
            local t = 1.0 - distSq / r2
            fx = fx + dx * t
            fy = fy + dy * t
        end
    end

    local lenSq = fx * fx + fy * fy
    if lenSq < 1e-6 then
        return 0, 0
    end

    -- ⭐ 关键：去掉 intent 前向分量（不影响朝向）
    local ix, iy = ctx.intent.dirX, ctx.intent.dirY
    local ilenSq = ix * ix + iy * iy
    if ilenSq > 1e-6 then
        local dot = fx * ix + fy * iy
        fx = fx - dot * ix
        fy = fy - dot * iy
    end

    local len = math.sqrt(fx * fx + fy * fy)
    if len < 1e-6 then
        return 0, 0
    end

    return (fx / len) * ctx.weight,
           (fy / len) * ctx.weight
end

----------------------------------------------------------------
-- System Update
----------------------------------------------------------------
---@param world World
---@param dt number
function SteeringSystem:update(world, dt)
    local C = _G.ComponentRegistry

    local steerings   = world:GetComponentOfType(C.Steering)
    local seeks       = world:GetComponentOfType(C.Seek)
    local separations = world:GetComponentOfType(C.Separation)
    local positions   = world:GetComponentOfType(C.Position)
    local intents     = world:GetComponentOfType(C.MoveIntent)

    local targetPos = positions[world.player_eid]

    for eid, steering in pairs(steerings) do
        steering.sepFx, steering.sepFy = 0, 0

        local pos    = positions[eid]
        local seek   = seeks[eid]
        local sep    = separations[eid]
        local intent = intents[eid]

        if not (pos and seek and sep and intent) then
            goto continue
        end

        -- 1️⃣ Seek：决定朝向 / 意图
        ApplySeek(seek, pos, targetPos, intent)

        -- todo 性能优化
        -- 2️⃣ Separation：只算位移修正
        local neighbors = CollectSeparationNeighbors(
            world, pos, eid, sep.radius
        )

        if #neighbors > 0 then
            local ctx = {
                pos       = pos,
                intent    = intent,
                radius    = sep.radius,
                weight    = sep.weight,
                neighbors = neighbors,
            }

            steering.sepFx, steering.sepFy = ComputeSeparation(ctx)
        end

        ::continue::
    end
end

return SteeringSystem
