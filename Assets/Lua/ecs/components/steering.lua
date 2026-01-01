--- SeparationComponent 用于处理实体的分离行为
--- @class SteeringComponent
--- @field fx number
--- @field fy number
--- @field sepFx number
--- @field sepFy number
--- @field maxForce number
Steering = {
    -- Separation 产生的速度修正（不是 force）
    fx = 0,
    fy = 0,                 -- 总 steering force
    sepFx = 0,
    sepFy = 0,              -- 纯分离（只影响速度）
    maxForce = 10,
}

return Steering
