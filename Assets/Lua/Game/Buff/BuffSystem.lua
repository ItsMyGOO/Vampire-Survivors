---
--- Created by echo.
--- DateTime: 2025/12/21 11:05
---
local BuffSystem = {
    buffs = {}
}

function BuffSystem:add(buff)
    table.insert(self.buffs, buff)
end

function BuffSystem:trigger(event, ctx)
    for _, buff in ipairs(self.buffs) do
        local handler = buff.hooks[event]
        if handler then
            handler(buff, ctx)
        end
    end
end

return BuffSystem
