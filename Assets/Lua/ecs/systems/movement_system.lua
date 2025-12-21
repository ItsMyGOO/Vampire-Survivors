---
--- Created by echo.
--- DateTime: 2025/12/21 21:46
---
local Velocity = require("components.velocity")
local Transform = require("components.transform")

local MovementSystem = {
    world = nil  -- 由 World 注入
}

--- 更新所有具有 Velocity 和 Transform 的实体
function MovementSystem:Update(dt)
    local vels = self.world.components[Velocity.__name]
    local transforms = self.world.components[Transform.__name]

    if not vels or not transforms then return end

    for eid, vel in pairs(vels) do
        local trans = transforms[eid]
        if trans and vel.active ~= false then
            trans.x = trans.x + vel.x * dt
            trans.y = trans.y + vel.y * dt
            trans.z = trans.z + vel.z * dt

            -- 同步到 Unity GameObject
            if trans.go then
                local pos = CS.UnityEngine.Vector3(trans.x, trans.y, trans.z)
                trans.go.transform.position = pos
            end
        end
    end
end

return MovementSystem