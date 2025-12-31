--- 分离组件，用于处理实体间的分离行为
--- @class SeparationComponent
--- @field public radius number 分离半径，定义需要避免重叠的距离范围
--- @field public weight number 分离权重，控制分离力的强度
local SeparationComponent = {
    radius = 1.2,
    weight = 1
}

return SeparationComponent
