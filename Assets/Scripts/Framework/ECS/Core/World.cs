using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Core
{
    /// <summary>
    /// ECS World —— 实体组件系统核心管理器。
    /// </summary>
    public class World
    {
        // 实体管理
        private int _nextEntityId = 1;
        private readonly HashSet<int> _activeEntities = new HashSet<int>();
        private readonly Queue<int>   _entitiesToDestroy = new Queue<int>();

        // 组件存储：Type -> 对应类型的 ComponentStore<T>
        private readonly Dictionary<Type, IComponentStore> _componentStores
            = new Dictionary<Type, IComponentStore>();

        // 系统列表
        private readonly List<ISystem> _systems = new List<ISystem>();

        // 服务注册表
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public int EntityCount => _activeEntities.Count;
        public int SystemCount => _systems.Count;

        // ============================================
        // 服务注册表
        // ============================================

        public void RegisterService<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
        }

        public bool TryGetService<T>(out T service) where T : class
        {
            foreach (var obj in _services.Values)
            {
                if (obj is T t)
                {
                    service = t;
                    return true;
                }
            }
            service = null;
            return false;
        }

        // ============================================
        // 实体管理
        // ============================================

        public int CreateEntity()
        {
            int id = _nextEntityId++;
            _activeEntities.Add(id);
            return id;
        }

        /// <summary>延迟销毁，帧末统一执行。</summary>
        public void DestroyEntity(int entityId)
        {
            if (_activeEntities.Contains(entityId))
                _entitiesToDestroy.Enqueue(entityId);
        }

        public bool EntityExists(int entityId) => _activeEntities.Contains(entityId);

        // ============================================
        // 组件管理
        // ============================================

        public void AddComponent<T>(int entityId, T component) where T : new()
        {
            if (!_activeEntities.Contains(entityId))
            {
                Debug.LogWarning($"尝试给不存在的实体 {entityId} 添加组件 {typeof(T).Name}");
                return;
            }
            GetOrCreateStore<T>().Add(entityId, component);
        }

        public T GetComponent<T>(int entityId) where T : new()
        {
            return GetOrCreateStore<T>().Get(entityId);
        }

        /// <summary>
        /// 将修改后的 struct 组件写回存储。
        /// class 组件无需调用（引用直接修改），但调用也无副作用。
        /// </summary>
        public void SetComponent<T>(int entityId, T component) where T : new()
        {
            GetOrCreateStore<T>().Set(entityId, component);
        }

        public bool TryGetComponent<T>(int entityId, out T component) where T : new()
        {
            component = default;

            if (!_activeEntities.Contains(entityId))
                return false;

            if (!_componentStores.TryGetValue(typeof(T), out var store))
                return false;

            if (!store.HasComponent(entityId))
                return false;

            component = ((ComponentStore<T>)store).Get(entityId);
            return true;
        }

        public bool HasComponent<T>(int entityId) where T : new()
        {
            if (!_componentStores.TryGetValue(typeof(T), out var store))
                return false;
            return store.HasComponent(entityId);
        }

        public void RemoveComponent<T>(int entityId) where T : new()
        {
            GetOrCreateStore<T>().Remove(entityId);
        }

        /// <summary>
        /// 返回可枚举视图，支持 foreach (var (entityId, component) in world.GetComponents&lt;T&gt;())。
        /// 零额外分配；迭代期间不得增删该类型组件。
        /// </summary>
        public ComponentStoreView<T> GetComponents<T>() where T : new()
        {
            return new ComponentStoreView<T>(GetOrCreateStore<T>());
        }

        /// <summary>
        /// 暴露紧凑裸数组供热路径 System 直接按索引遍历（最低开销）。
        /// 迭代期间不得增删该类型组件。
        /// </summary>
        public void IterateComponents<T>(out int[] ids, out T[] data, out int count) where T : new()
        {
            GetOrCreateStore<T>().GetRawArrays(out ids, out data, out count);
        }

        /// <summary>
        /// 将拥有某组件的所有实体 ID 填入外部缓冲区（零 GC）。
        /// </summary>
        public void GetEntitiesWithComponent<T>(List<int> buffer) where T : new()
        {
            if (!_componentStores.TryGetValue(typeof(T), out var store))
            {
                buffer.Clear();
                return;
            }
            store.FillEntityBuffer(buffer);
        }

        // ============================================
        // 系统管理
        // ============================================

        public void RegisterSystem(ISystem system)
        {
            if (!_systems.Contains(system))
                _systems.Add(system);
        }

        public void UnregisterSystem(ISystem system)
        {
            _systems.Remove(system);
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _systems.Count; i++)
                _systems[i].Update(this, deltaTime);

            ProcessDestroyQueue();
        }

        // ============================================
        // 内部方法
        // ============================================

        private ComponentStore<T> GetOrCreateStore<T>() where T : new()
        {
            var type = typeof(T);
            if (!_componentStores.TryGetValue(type, out var store))
            {
                var newStore = new ComponentStore<T>();
                _componentStores[type] = newStore;
                return newStore;
            }
            return (ComponentStore<T>)store;
        }

        private void ProcessDestroyQueue()
        {
            while (_entitiesToDestroy.Count > 0)
            {
                int entityId = _entitiesToDestroy.Dequeue();

                if (!_activeEntities.Contains(entityId))
                    continue;

                _activeEntities.Remove(entityId);

                foreach (var store in _componentStores.Values)
                {
                    if (store.HasComponent(entityId))
                        store.RemoveComponent(entityId);
                }
            }
        }

        // ============================================
        // 调试和统计
        // ============================================

        public List<int> GetAllEntities()
        {
            return new List<int>(_activeEntities);
        }

        public Dictionary<string, int> GetComponentStats()
        {
            var stats = new Dictionary<string, int>();
            foreach (var kvp in _componentStores)
                stats[kvp.Key.Name] = kvp.Value.GetComponentCount();
            return stats;
        }

        public void DebugPrint()
        {
            Debug.Log("=== ECS World 状态 ===");
            Debug.Log($"实体数量: {EntityCount}");
            Debug.Log($"系统数量: {SystemCount}");
            Debug.Log($"组件类型数量: {_componentStores.Count}");
            Debug.Log("\n组件统计:");
            foreach (var kvp in GetComponentStats())
                Debug.Log($"  {kvp.Key}: {kvp.Value}");
        }

        public void Clear()
        {
            _activeEntities.Clear();
            _entitiesToDestroy.Clear();
            _componentStores.Clear();
            _nextEntityId = 1;
        }
    }
}