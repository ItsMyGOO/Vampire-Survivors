---
--- Created by echo.
--- DateTime: 2026/1/13 21:51
---
local UpgradeFlow = {}

--- C# 调用入口
--- @param options table<number, UpgradeOption>
function UpgradeFlow.OnUpgradeOptions(options)
    if options == nil or #options == 0 then
        print("[UpgradeFlow] no upgrade options")
        return
    end

    -- 随机选一个（替代 UI）
    local index = math.random(1, #options)
    local selected = options[index]

    print(string.format(
        "[UpgradeFlow] auto select [%s] %s",
        selected.type,
        selected.name
    ))

    -- 调用 C# Apply
    CS.Battle.UpgradeApplyService.Apply(
        selected
    )
end

return UpgradeFlow
