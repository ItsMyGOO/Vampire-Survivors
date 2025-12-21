---
--- Created by echo.
--- DateTime: 2025/12/21 22:32
---
---@meta

--- CSharp Interop Namespace (Injected by xLua/ToLua at runtime)
---
---@class CSNamespace
CS = {}

---@class UnityEngine
CS.UnityEngine = {}

---@class UnityEngine.Vector3
---@field public x number
---@field public y number
---@field public z number
---@overload fun(x: number, y: number, z: number): UnityEngine.Vector3
CS.UnityEngine.Vector3 = {}

-- 添加常用类...
---@class UnityEngine.GameObject
---@field public name string
---@field public transform UnityEngine.Transform
CS.UnityEngine.GameObject = {}

---@class UnityEngine.Behaviour
---@field public enabled boolean
CS.UnityEngine.Behaviour = {}