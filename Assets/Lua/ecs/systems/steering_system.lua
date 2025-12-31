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
---@param grid Grid
---@param saparation SeparationComponent
---@param pos PositionComponent
local function ApplySeparation(selfId, steering, grid, saparation, pos)
    local fx, fy = 0.0, 0.0
    local count = 0

    local neighbors = grid:query(pos.x, pos.y, saparation.radius)

    for eid, n in ipairs(neighbors) do
        if eid ~= selfId and n.Position then
            local dx = pos.x - n.Position.x
            local dy = pos.y - n.Position.y
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
    for eid, steering in pairs(steerings) do
        --重置
        steering.fx, steering.fy = 0, 0
        --seek
        local pos = positions[eid]
        local seek = seeks[eid]
        ApplySeek(steering, seek, pos, target)

        local separation = separations[eid]
        ApplySeparation(eid, steering, world.grid, separation, pos)
    end


    for _, e in pairs(self.entities) do
        local pos = e.Position
        local sep = e.Separation
        local steer = e.Steering

        local fx, fy = 0, 0
        local count = 0

        local neighbors = Grid:query(pos.x, pos.y, sep.radius)

        for _, n in ipairs(neighbors) do
            if n ~= e then
                local dx = pos.x - n.Position.x
                local dy = pos.y - n.Position.y
                local distSq = dx * dx + dy * dy

                if distSq > 0 and distSq < sep.radius * sep.radius then
                    local dist = math.sqrt(distSq)
                    fx = fx + dx / dist
                    fy = fy + dy / dist
                    count = count + 1
                end
            end
        end

        if count > 0 then
            fx = fx / count * sep.weight
            fy = fy / count * sep.weight
        end

        steer.fx = steer.fx + fx
        steer.fy = steer.fy + fy
    end
end

return SteeringSystem
