---
--- Created by echo.
--- DateTime: 2026/1/5 17:17
---
-- Core/Buff/BuffManager.lua
local BuffInstance = require("buff.buff_instance")
local BuffExecutor = require("buff.buff_executor")

local BuffManager = {}
BuffManager.__index = BuffManager

function BuffManager.new(player)
    return setmetatable({
        player = player,
        buffs = {},
    }, BuffManager)
end

function BuffManager:AddBuff(def)
    local inst = self.buffs[def.id]
    if inst then
        if inst.stack < (def.maxStack or 1) then
            inst.stack = inst.stack + 1
            print("[Buff] Stack", def.id, inst.stack)
            BuffExecutor.Execute(def.onAdd, self.player, nil, inst.stack)
        end
    else
        inst = BuffInstance.new(def)
        self.buffs[def.id] = inst
        print("[Buff] Add", def.id)
        BuffExecutor.Execute(def.onAdd, self.player, nil, inst.stack)
    end
end

function BuffManager:Trigger(triggerName, ctx)
    for _, inst in pairs(self.buffs) do
        local effects = inst.def.onTrigger and inst.def.onTrigger[triggerName]
        BuffExecutor.Execute(effects, self.player, ctx, inst.stack)
    end
end

return BuffManager
