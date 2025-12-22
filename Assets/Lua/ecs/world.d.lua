---
--- Created by echo.
--- DateTime: 2025/12/22 20:27
---
---@class World
---@field public entities table<integer, boolean>
---@field public components table<string, table<integer, table>>
---@field public eventBus EventBus?
---@field public schemaNames table<table, string>
--
---@field public AddEntity fun(self: World): integer
---@field public AddComponent fun(self: World, eid: integer, schema: table, override?: table): nil
---@field public GetComponent fun(self: World, eid: integer, schema: table): table?
---@field public HasComponent fun(self: World, eid: integer, schema: table): boolean
---@field public RemoveComponent fun(self: World, eid: integer, schema: table): nil
---@field public DestroyEntity fun(self: World, eid: integer): nil
---@field private getSchemaName fun(self: World, schema: table): string
---@field private deepCopy fun(self: World, orig: table): table

return {}
