---
--- Created by echo.
--- DateTime: 2025/12/21 22:32
---
---@meta

--- CSharp Interop Namespace (Injected by xLua/ToLua at runtime)
---
---@class CS
CS = {}

---@class CS.UnityEngine
CS.UnityEngine = {}

---@class CS.UnityEngine.Debug
CS.UnityEngine.Debug = {}

---@param msg string
function CS.UnityEngine.Debug.Log(msg) end

---@class CS.UnityEngine.Input
CS.UnityEngine.Input = {}
---@param msg string
---@return number
function CS.UnityEngine.Input.GetAxisRaw(msg) end

---@class CS.UnityEngine.Vector3
---@field public x number
---@field public y number
---@field public z number
---@overload fun(x: number, y: number, z: number): UnityEngine.Vector3
CS.UnityEngine.Vector3 = {}

---@class CS.UnityEngine.Object
CS.UnityEngine.Object = {}

--- GameObject 类型定义扩展
---@class CS.UnityEngine.GameObject : CS.UnityEngine.Object
---@field public transform CS.UnityEngine.Transform
---@field public activeSelf boolean
---@field public activeInHierarchy boolean
---@field public name string
---@field public tag string
---@field public layer integer
CS.UnityEngine.GameObject = {}

---@param self CS.UnityEngine.GameObject
---@param active boolean
function CS.UnityEngine.GameObject.SetActive(self, active) end

---@overload fun(original: CS.UnityEngine.GameObject): CS.UnityEngine.GameObject
---@overload fun(original: CS.UnityEngine.GameObject, parent: CS.UnityEngine.Transform): CS.UnityEngine.GameObject
---@overload fun(original: CS.UnityEngine.GameObject, position: CS.UnityEngine.Vector3, rotation: CS.UnityEngine.Quaternion): CS.UnityEngine.GameObject
---@overload fun(original: CS.UnityEngine.GameObject, position: CS.UnityEngine.Vector3, rotation: CS.UnityEngine.Quaternion, parent: CS.UnityEngine.Transform): CS.UnityEngine.GameObject
---@return CS.UnityEngine.GameObject
function CS.UnityEngine.GameObject.Instantiate(original, param1, param2, param3) end

---@class CS.UnityEngine.Component : CS.UnityEngine.Object
CS.UnityEngine.Component = {}

---@class UnityEngine.Transform
---@field public position UnityEngine.Vector3
---@field public rotation UnityEngine.Quaternion
---@field public localScale UnityEngine.Vector3
---@field public parent UnityEngine.Transform
---@field public childCount integer
CS.UnityEngine.Transform = {}
