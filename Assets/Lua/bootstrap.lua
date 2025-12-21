---
--- Created by echo.
--- DateTime: 2025/12/21 12:18
---
print("lua bootstrap start")

-- 可选：全局 require 保护
local ok, err = pcall(function()
    require("Game.Skill.Fireball")
end)

if not ok then
    print("lua error:", err)
end
