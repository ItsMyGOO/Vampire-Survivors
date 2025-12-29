---
--- Created by echo.
--- DateTime: 2025/12/28 23:37
---
local BaseSystem = require("ecs.base_system")

---@class RenderSystem : BaseSystem
local RenderSystem = {
    ---@type table<number,SpriteKeyComponent>?
    sprites = nil,
    ---@type table<number,RenderHandlerComponent>?
    renderer = nil,
}
RenderSystem.__index = RenderSystem

---@return RenderSystem
function RenderSystem.new()
    local self = BaseSystem.new()
    return setmetatable(self, RenderSystem)
end

---@param world World
function RenderSystem:start(world)
    BaseSystem.start(self, world)

    local SpriteKey = require("ecs.component.sprite_key")
    local Renderer = require("ecs.component.render_handler")

    self.sprites = world:GetComponentOfType(SpriteKey)
    self.renderer = world:GetComponentOfType(Renderer)
end

function RenderSystem:update(dt)
    if (self.sprites and self.renderer) then
        for _, sprite in pairs(self.sprites) do
            local sr = sprite.SpriteRendererComponent.renderer

            if sprite and sr.sprite ~= sprite then
                sr.sprite = sprite
            end
        end
    end
end

function RenderSystem:shutdown()
    self.transforms = nil
end

return RenderSystem
