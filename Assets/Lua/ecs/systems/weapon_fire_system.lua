---
--- Created by echo.
--- DateTime: 2026/1/7 16:57
---
-- systems/VSWeaponFireSystem.lua
local WeaponDefs           = require("Data.weapon_defs")
local Pipeline             = require("weapon.weapon_pipeline")

local VSWeaponFireSystem   = {}
VSWeaponFireSystem.__index = VSWeaponFireSystem

function VSWeaponFireSystem:update(world, dt)
    local C          = _G.ComponentRegistry

    local players    = world:GetComponentOfType(C.PlayerTag)
    local slotsComps = world:GetComponentOfType(C.WeaponSlots)

    for owner, _ in pairs(players) do
        local slotsComp = slotsComps[owner]
        if not slotsComp then goto continue_player end

        for _, slot in ipairs(slotsComp.slots) do
            local def = WeaponDefs[slot.def]
            if not def then goto continue_slot end

            -- 根据武器类型分流
            if def.type == "Projectile" then
                slot.timer = slot.timer or 0
                slot.timer = slot.timer - dt
                if slot.timer > 0 then goto continue_slot end

                local ctx = {
                    world     = world,
                    owner     = owner,
                    slot      = slot,
                    weaponDef = def,

                    -- pipeline 初始值
                    interval  = def.interval,
                    count     = def.base_count,
                    damage    = def.base_damage,
                    speed     = def.base_speed,
                    range     = def.range,
                }

                Pipeline.Execute(ctx)

                slot.timer = ctx.interval or def.interval
            elseif def.type == "Orbit" then
                -- Orbit 只生成一次
                if not slot.spawned then
                    local ctx = {
                        world     = world,
                        owner     = owner,
                        slot      = slot,
                        weaponDef = def,
                    }

                    Pipeline.Execute(ctx)
                    slot.spawned = true
                end
            end

            ::continue_slot::
        end

        ::continue_player::
    end
end

return VSWeaponFireSystem
