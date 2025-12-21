---
--- Created by echo.
--- DateTime: 2025/12/21 21:41
---
local Entity = {}
local lastId = 0

function Entity.Create()
    lastId = lastId + 1
    return lastId
end

return Entity