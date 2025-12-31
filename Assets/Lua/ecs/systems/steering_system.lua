---
--- Created by echo.
--- DateTime: 2025/12/31 16:01
---
---@class SteeringSystem
local SteeringSystem = {}
SteeringSystem.__index = SteeringSystem

---@param steer SteeringComponent
---@param seek SeekComponent
---@param pos PositionComponent
---@param target PositionComponent
local function ApplySeek(steer, seek, pos, target)
    local dx = target.x - pos.x
    local dy = target.y - pos.y
    local dist = math.sqrt(dx * dx + dy * dy)

    if dist < 0.001 then return end

    dx = dx / dist
    dy = dy / dist

    -- 可选减速
    if seek.slowingRadius and seek.slowingRadius > 0 and dist < seek.slowingRadius then
        local scale = dist / seek.slowingRadius
        dx = dx * scale
        dy = dy * scale
    end

    steer.fx = steer.fx + dx * seek.weight
    steer.fy = steer.fy + dy * seek.weight
end

---@param selfId integer
---@param steering SteeringComponent
---@param grid Grid?
---@param saparation SeparationComponent
---@param pos PositionComponent
local function ApplySeparation(selfId, steering, grid, saparation, pos)
    local fx, fy = 0.0, 0.0
    local count = 0

    if not grid then
        return
    end
    local neighbors = grid:query(pos.x, pos.y, saparation.radius)

    for eid, nPos in ipairs(neighbors) do
        if eid ~= selfId and nPos then
            local dx = pos.x - nPos.x
            local dy = pos.y - nPos.y
            local distSq = dx * dx + dy * dy

            if distSq > 0 and distSq < saparation.radius * saparation.radius then
                local dist = math.sqrt(distSq)
                fx = fx + dx / dist
                fy = fy + dy / dist
                count = count + 1
            end
        end
    end

    if count > 0 then
        fx = fx / count * saparation.weight
        fy = fy / count * saparation.weight
        steering.fx = steering.fx + fx
        steering.fy = steering.fy + fy
    end
end

---@param world World
---@param dt number
function SteeringSystem:update(world, dt)
    local ComponentRegistry = _G.ComponentRegistry

    ---@type table<integer, SteeringComponent>
    local steerings = world:GetComponentOfType(ComponentRegistry.Steering)
    local seeks = world:GetComponentOfType(ComponentRegistry.Seek)
    local separations = world:GetComponentOfType(ComponentRegistry.Separation)
    local positions = world:GetComponentOfType(ComponentRegistry.Position)

    local target = positions[world.player_eid]
    for eid, steer in pairs(steerings) do
        --重置
        steer.fx, steer.fy = 0.0, 0.0
        -- seek
        local pos = positions[eid]
        local seek = seeks[eid]
        ApplySeek(steer, seek, pos, target)

        -- separation
        local separation = separations[eid]
        ApplySeparation(eid, steer, world.grid, separation, pos)

        -- 最大force
        local fx = steer.fx
        local fy = steer.fy

        local len = math.sqrt(fx * fx + fy * fy)
        if len > steer.maxForce then
            fx = fx / len * steer.maxForce
            fy = fy / len * steer.maxForce
        end

        steer.fx = fx
        steer.fy = fy
    end
end

return SteeringSystem
