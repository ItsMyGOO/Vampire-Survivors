---
--- Created by echo.
--- DateTime: 2025/12/29 21:19
---
local ComponentRegistry = require("ecs.component_registry")
_G.ComponentRegistry = ComponentRegistry

local AnimConfigHandler = require("ConfigHandler.anim_config_handler")
_G.AnimConfigHandler = AnimConfigHandler

local Battle = require("battle.battle")
_G.Battle = Battle

local RenderBridge = require('Presentation/lua_render_bridge')
 _G.RenderBridge = RenderBridge
