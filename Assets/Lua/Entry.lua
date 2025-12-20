---
--- Created by echo.
--- DateTime: 2025/12/20 16:27
---

print("Hello XLua")

function Add(a, b)
    return a + b
end

function CallCS(obj)
    obj:HelloFromLua()
end
