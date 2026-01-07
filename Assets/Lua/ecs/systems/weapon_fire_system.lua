---
--- Created by echo.
--- DateTime: 2026/1/7 16:57
---
-- systems/VSWeaponFireSystem.lua
local WeaponDefs = require("Data.weapon_defs")
local Pipeline   = require("weapon.weapon_pipeline")

local VSWeaponFireSystem = {}
VSWeaponFireSystem.__index = VSWeaponFireSystem

function VSWeaponFireSystem:update(world, dt)
    local C = _G.ComponentRegistry

    local slots = world:GetComponentOfType(C.WeaponSlot)

    for owner, slot in pairs(slots) do
        local def = WeaponDefs[slot.def]
        if not def then goto continue end

        slot.timer = slot.timer - dt
        if slot.timer > 0 then goto continue end

        local ctx = {
            world = world,
            owner = owner,
            weapon = def
        }

        Pipeline.Execute(ctx)

        slot.timer = ctx.interval or def.interval

        ::continue::
    end
end

return VSWeaponFireSystem
