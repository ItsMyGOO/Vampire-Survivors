---
--- Created by echo.
--- DateTime: 2025/12/21 21:41
---
--- 实体ID生成器
--- @class Entity
local Entity = {}
local lastId = 0

---
--- 创建一个新的实体ID
--- @return integer 新的实体ID
function Entity.Create()
    lastId = lastId + 1
    return lastId
end

return Entity
