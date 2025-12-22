---
--- Created by echo.
--- DateTime: 2025/12/22 16:23
---
---@class GameObjectLink
local Link = {
    go = nil,
    --- 当实体被销毁时自动清理 GameObject
    dispose = function(self)
        if self.go then
            CS.UnityEngine.Object.Destroy(self.go)
        end
    end
}