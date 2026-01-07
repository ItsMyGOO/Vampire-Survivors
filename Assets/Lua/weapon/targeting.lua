---
--- Created by echo.
--- DateTime: 2026/1/6 17:54
---
local Targeting = {}

function Targeting.FindNearest(pos, range, count)
    local enemies = ECS.QueryEnemies(pos, range)

    table.sort(enemies, function(a, b)
        return (a.pos - pos):sqrMagnitude() < (b.pos - pos):sqrMagnitude()
    end)

    local result = {}
    for i = 1, math.min(count, #enemies) do
        result[#result + 1] = enemies[i]
    end

    return result
end

return Targeting
