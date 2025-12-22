---
--- Created by echo.
--- DateTime: 2025/12/22 16:40
---

---
--- 高性能事件总线（Event Bus）
--- 支持：发布/订阅、优先级、自动清理、通配符匹配
--- 使用方式：
---   local bus = require("utils.event_bus").new()
---   bus:on("player_died", function(data) ... end, priority)
---   bus:emit("enemy_killed", { id = 123 })
---

---
--- 默认比较器，用于按优先级排序
--- @param a table 监听器A
--- @param b table 监听器B
--- @return boolean A的优先级是否小于B的优先级
local function defaultComparator(a, b)
    return a.priority < b.priority
end

---
--- 创建一个新的 Event Bus
--- @return EventBus
return function()
    ---
    --- 事件总线类
    --- @class EventBus
    --- @field private listeners table 事件名 → 监听器列表
    --- @field private isDispatching boolean 是否正在派发事件（用于安全移除）
    --- @field private toRemove table 待清理的监听器（在派发后清理）
    local bus = {
        ---@private
        listeners = {},
        ---@private
        isDispatching = false,
        ---@private
        toRemove = {}
    }

    ---
    --- 添加事件监听
    --- @param eventName string 事件名，支持通配符 "*"（如 "on_*"）
    --- @param callback fun(data: any) 回调函数
    --- @param priority number? 可选，数值越小越早执行
    --- @return table ListenerHandle 句柄，可用于取消监听
    function bus:on(eventName, callback, priority)
        assert(type(eventName) == "string", "eventName must be string")
        assert(type(callback) == "function", "callback must be function")

        priority = priority or 0

        local listener = {
            event = eventName,
            callback = callback,
            priority = priority,
            enabled = true
        }

        if not bus.listeners[eventName] then
            bus.listeners[eventName] = {}
        end

        table.insert(bus.listeners[eventName], listener)

        -- 按优先级排序（插入后重排，小项目可用；大项目可用堆优化）
        table.sort(bus.listeners[eventName], defaultComparator)

        -- 返回句柄用于 off
        return listener
    end

    ---
    --- 移除事件监听
    --- @param handle table ListenerHandle 由 on() 返回的句柄
    function bus:off(handle)
        if not handle or not handle.event then return end

        local list = bus.listeners[handle.event]
        if not list then return end

        if bus.isDispatching then
            -- 延迟清理，避免遍历时修改列表
            bus.toRemove[#bus.toRemove + 1] = handle
        else
            for i, l in ipairs(list) do
                if l == handle then
                    table.remove(list, i)
                    break
                end
            end
        end
    end

    ---
    --- 触发事件（同步）
    --- @param eventName string 事件名
    --- @param data any 事件数据
    function bus:emit(eventName, data)
        bus.isDispatching = true

        -- 精确匹配
        local exactListeners = bus.listeners[eventName]
        if exactListeners then
            for _, listener in ipairs(exactListeners) do
                if listener.enabled then
                    xpcall(listener.callback, debug.traceback, data)
                end
            end
        end

        -- 通配符匹配：如 "on_enemy_killed" 匹配 "on_*"
        for pattern, listeners in pairs(bus.listeners) do
            if pattern ~= eventName and pattern:sub(-1) == "*" then
                local prefix = pattern:sub(1, -2)
                if eventName:find("^" .. prefix) then
                    for _, listener in ipairs(listeners) do
                        if listener.enabled then
                            xpcall(listener.callback, debug.traceback, data)
                        end
                    end
                end
            end
        end

        bus.isDispatching = false

        -- 清理延迟移除的监听器
        if #bus.toRemove > 0 then
            for _, handle in ipairs(bus.toRemove) do
                self:offImmediate(handle)
            end
            bus.toRemove = {}
        end
    end

    ---
    --- 立即移除监听器（不推荐外部调用）
    --- @private
    --- @param handle table ListenerHandle 监听器句柄
    function bus:offImmediate(handle)
        local list = bus.listeners[handle.event]
        if not list then return end
        for i = #list, 1, -1 do
            if list[i] == handle then
                table.remove(list, i)
                break
            end
        end
    end

    ---
    --- 清空所有监听器（重启时使用）
    function bus:clear()
        bus.listeners = {}
        bus.toRemove = {}
    end

    ---
    --- 获取监听器数量（调试用）
    --- @return integer 监听器总数
    function bus:listenerCount()
        local count = 0
        for _, list in pairs(bus.listeners) do
            count = count + #list
        end
        return count
    end

    return bus
end
