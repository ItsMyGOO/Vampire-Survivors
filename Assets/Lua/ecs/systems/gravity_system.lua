---
--- Created by echo.
--- DateTime: 2025/12/22 16:28
---
-- systems/gravity_system.lua
---@class GravitySystem
local GravitySystem = {
    world = nil,
    _transforms = nil,
    _fallingBodies = nil
}

function GravitySystem:start(world)
    self.world = assert(world)
    self._transforms = world.components[require('Assets.Lua.ecs.components.position').__name]
    self._fallingBodies = world.components[require("ecs.components.falling_body").__name]
end

function GravitySystem:update(dt)
    if not self._transforms or not self._fallingBodies then return end

    local transforms = self._transforms
    local bodies = self._fallingBodies

    for eid, body in pairs(bodies) do
        if body.isFalling and not body.hasLanded then
            local trans = transforms[eid]
            if trans then
                trans.y = trans.y - body.speed * dt

                if trans.y <= body.targetY then
                    trans.y = body.targetY
                    body.hasLanded = true
                    body.isFalling = false

                    -- 触发“已落地”事件
                    self.world:event("on_fall_landed", eid, trans.x, trans.y, trans.z)
                end

                -- 同步到 Unity
                if trans.go then
                    trans.go.transform.position = CS.UnityEngine.Vector3(trans.x, trans.y, trans.z)
                end
            end
        end
    end
end

return GravitySystem