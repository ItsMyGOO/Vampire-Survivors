---
--- Created by echo.
--- DateTime: 2026/1/6 17:46
---
local ECSBridge = require("ecs_bridge")
local Targeting = require("targeting")

local WeaponInstance = {}
WeaponInstance.__index = WeaponInstance

function WeaponInstance.new(owner, def)
    return setmetatable({
        owner = owner,
        def = def,
        timer = 0,
        spawned = false
    }, WeaponInstance)
end

function WeaponInstance:Update(dt)
    -- 环绕型武器：只生成一次
    if self.def.type == "orbit" then
        if not self.spawned then
            self:SpawnOrbit()
            self.spawned = true
        end
        return
    end

    -- 发射型武器
    self.timer = self.timer + dt
    if self.timer < self.def.interval then
        return
    end

    self.timer = self.timer - self.def.interval
    self:FireProjectile()
end

function WeaponInstance:FireProjectile()
    local targets = Targeting.FindNearest(
        self.owner.pos,
        self.def.range,
        self.def.count
    )

    for _, target in ipairs(targets) do
        local dir = (target.pos - self.owner.pos):normalized()

        ECSBridge.SpawnProjectile({
            owner     = self.owner,
            position  = self.owner.pos,
            direction = dir,
            speed     = self.def.speed,
            damage    = self.def.damage,
        })
    end
end

function WeaponInstance:SpawnOrbit()
    for i = 1, self.def.count do
        ECSBridge.SpawnOrbit({
            owner        = self.owner,
            radius       = self.def.radius,
            angularSpeed = self.def.angularSpeed * (i % 2 == 0 and 1 or -1),
            damage       = self.def.damage,
        })
    end
end

return WeaponInstance
