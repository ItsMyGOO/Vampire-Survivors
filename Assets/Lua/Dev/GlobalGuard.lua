---
--- Created by echo.
--- DateTime: 2025/12/21 13:30
---
-- Lua/Dev/GlobalGuard.lua
-- 开发期全局变量写入探针

local M = {}

-- 白名单（允许存在的全局）
local whitelist = {
    _G = true,
    _VERSION = true,

    -- Lua 标准库
    print = true,
    ipairs = true,
    pairs = true,
    next = true,
    tonumber = true,
    tostring = true,
    type = true,
    error = true,
    assert = true,

    -- table / string / math / coroutine / os
    table = true,
    string = true,
    math = true,
    coroutine = true,
    os = true,

    -- XLua / Unity 常见
    CS = true,
    Game = true,
}

-- 已声明的全局（启动时快照）
local declared = {}

local function snapshot_globals()
    for k, _ in pairs(_G) do
        declared[k] = true
    end
end

local function report(key)
    local info = debug.getinfo(3, "Sl")
    local src = info and info.short_src or "unknown"
    local line = info and info.currentline or 0

    error(string.format(
        "[GlobalGuard] illegal global write: '%s'\n  at %s:%d",
        tostring(key), src, line
    ), 2)
end

function M.enable()
    snapshot_globals()

    setmetatable(_G, {
        __newindex = function(_, key, value)
            if not declared[key] and not whitelist[key] then
                report(key)
            end
            rawset(_G, key, value)
        end
    })
end

function M.disable()
    setmetatable(_G, nil)
end

return M
