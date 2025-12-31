---
--- Created by echo.
--- DateTime: 2025/12/31 16:01
---
---@class SteeringSystem
local SteeringSystem = {}
SteeringSystem.__index = SteeringSystem

---@param seek SeekComponent
---@param pos PositionComponent
---@param target PositionComponent
---@param intent MoveIntentComponent
local function ApplySeek(seek, pos, target, intent)
    local dx = target.x - pos.x
    local dy = target.y - pos.y
    local dist = math.sqrt(dx * dx + dy * dy)

    if dist < seek.stop_distance then
        intent.dirX = 0
        intent.dirY = 0
        return
    end

    dx = dx / dist
    dy = dy / dist

    intent.dirX = dx
    intent.dirY = dy
    intent.speed = seek.speed
end


---@param selfId integer
---@param steering SteeringComponent
---@param grid Grid?
---@param separation SeparationComponent
---@param pos PositionComponent
---@param intent MoveIntentComponent
local function ApplySeparation(selfId, steering, grid, separation, pos, intent)
    if not grid then return end

    local fx, fy = 0.0, 0.0
    local count = 0

    local neighbors = grid:query(pos.x, pos.y, separation.radius)

    for _, nPos in ipairs(neighbors) do
        if nPos.eid ~= selfId then
            local dx = pos.x - nPos.x
            local dy = pos.y - nPos.y
            local distSq = dx * dx + dy * dy

            if distSq > 0 and distSq < separation.radius * separation.radius then
                local dist = math.sqrt(distSq)
                fx = fx + dx / dist
                fy = fy + dy / dist
                count = count + 1
            end
        end
    end

    if count == 0 then
        steering.sepVx = 0
        steering.sepVy = 0
        return
    end

    -- 平均方向
    fx = fx / count
    fy = fy / count

    -- 核心：去掉前进方向分量（只保留横向）
    local ix, iy = intent.dirX, intent.dirY
    local dot = fx * ix + fy * iy
    fx = fx - dot * ix
    fy = fy - dot * iy

    -- 归一化
    local len = math.sqrt(fx * fx + fy * fy)
    if len > 0 then
        fx = fx / len
        fy = fy / len
    end

    steering.sepVx = fx * separation.weight
    steering.sepVy = fy * separation.weight
end

---@param world World
---@param dt number
function SteeringSystem:update(world, dt)
    local C           = _G.ComponentRegistry

    local steerings   = world:GetComponentOfType(C.Steering)
    local seeks       = world:GetComponentOfType(C.Seek)
    local separations = world:GetComponentOfType(C.Separation)
    local positions   = world:GetComponentOfType(C.Position)
    local intents     = world:GetComponentOfType(C.MoveIntent)

    local targetPos   = positions[world.player_eid]

    for eid, steer in pairs(steerings) do
        steer.fx, steer.fy = 0, 0
        steer.sepFx, steer.sepFy = 0, 0

        local pos = positions[eid]

        -- seek → 主方向
        local seek = seeks[eid]
        local intent = intents[eid]
        ApplySeek(seek, pos, targetPos,intent)

        -- separation → 只算分离
        local sep = separations[eid]
        ApplySeparation(eid, steer, world.grid, sep, pos,intent)

        -- clamp 总 steering（不 clamp sep）
        local len = math.sqrt(steer.fx * steer.fx + steer.fy * steer.fy)
        if len > steer.maxForce then
            steer.fx = steer.fx / len * steer.maxForce
            steer.fy = steer.fy / len * steer.maxForce
        end
    end
end

return SteeringSystem
