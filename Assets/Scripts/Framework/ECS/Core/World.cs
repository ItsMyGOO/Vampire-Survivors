using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Core
{
    /// <summary>
    /// ECS World - 实体组件系统的核心管理器
    /// 职责：
    /// 1. 实体生命周期管理（创建/销毁）
    /// 2. 组件存储管理
    /// 3. 系统注册和更新
    /// </summary>
    public class World
    {
        // 实体管理
        private int nextEntityId = 1;
        private HashSet<int> activeEntities = new HashSet<int>();
        private Queue<int> entitiesToDestroy = new Queue<int>();

        // 组件存储
        private Dictionary<Type, IComponentStore> componentStores = new Dictionary<Type, IComponentStore>();

        // 系统管理
        private List<ISystem> systems = new List<ISystem>();

        // 统计信息
        public int EntityCount => activeEntities.Count;
        public int SystemCount => systems.Count;

        private Dictionary<Type, object> services = new();

        public void RegisterService<T>(T service) where T : class
        {
            services[typeof(T)] = service;
        }

        public bool TryGetService<T>(out T service) where T : class
        {
            foreach (var obj in services.Values)
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

        /// <summary>
        /// 创建新实体
        /// </summary>
        public int CreateEntity()
        {
            int id = nextEntityId++;
            activeEntities.Add(id);
            return id;
        }

        /// <summary>
        /// 销毁实体（延迟销毁，在帧结束时执行）
        /// </summary>
        public void DestroyEntity(int entityId)
        {
            if (activeEntities.Contains(entityId))
            {
                entitiesToDestroy.Enqueue(entityId);
            }
        }

        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        public bool EntityExists(int entityId)
        {
            return activeEntities.Contains(entityId);
        }

        // ============================================
        // 组件管理
        // ============================================

        /// <summary>
        /// 添加组件到实体
        /// </summary>
        public void AddComponent<T>(int entityId, T component) where T : class, new()
        {
            if (!activeEntities.Contains(entityId))
            {
                Debug.LogWarning($"尝试给不存在的实体 {entityId} 添加组件 {typeof(T).Name}");
                return;
            }

            var store = GetOrCreateStore<T>();
            store.Add(entityId, component);
        }

        /// <summary>
        /// 获取组件（返回引用，可修改）
        /// </summary>
        public T GetComponent<T>(int entityId) where T : class, new()
        {
            var store = GetOrCreateStore<T>();
            return store.Get(entityId);
        }

        /// <summary>
        /// 检查实体是否有某组件
        /// </summary>
        public bool HasComponent<T>(int entityId) where T : class, new()
        {
            if (!componentStores.TryGetValue(typeof(T), out var store))
            {
                return false;
            }

            return store.HasComponent(entityId);
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveComponent<T>(int entityId) where T : class, new()
        {
            var store = GetOrCreateStore<T>();
            store.Remove(entityId);
        }

        /// <summary>
        /// 获取所有拥有某组件的实体（用于系统遍历）
        /// </summary>
        public IEnumerable<KeyValuePair<int, T>> GetComponents<T>() where T : class, new()
        {
            var store = GetOrCreateStore<T>();
            return store.GetAll();
        }

        /// <summary>
        /// 获取所有拥有某组件的实体ID列表
        /// </summary>
        public List<int> GetEntitiesWithComponent<T>() where T : class, new()
        {
            var store = GetOrCreateStore<T>();
            var entities = new List<int>();

            foreach (var kvp in store.GetAll())
            {
                entities.Add(kvp.Key);
            }

            return entities;
        }

        // ============================================
        // 系统管理
        // ============================================

        /// <summary>
        /// 注册系统
        /// </summary>
        public void RegisterSystem(ISystem system)
        {
            if (!systems.Contains(system))
            {
                systems.Add(system);
            }
        }

        /// <summary>
        /// 移除系统
        /// </summary>
        public void UnregisterSystem(ISystem system)
        {
            systems.Remove(system);
        }

        /// <summary>
        /// 更新所有系统
        /// </summary>
        public void Update(float deltaTime)
        {
            // 更新所有系统
            foreach (var system in systems)
            {
                try
                {
                    system.Update(this, deltaTime);
                }
                catch (Exception e)
                {
                    Debug.LogError($"系统 {system.GetType().Name} 更新时出错: {e.Message}\n{e.StackTrace}");
                }
            }

            // 处理延迟销毁的实体
            ProcessDestroyQueue();
        }

        // ============================================
        // 内部方法
        // ============================================

        /// <summary>
        /// 获取或创建组件存储
        /// </summary>
        private ComponentStore<T> GetOrCreateStore<T>() where T : class, new()
        {
            var type = typeof(T);

            if (!componentStores.TryGetValue(type, out var store))
            {
                var newStore = new ComponentStore<T>();
                componentStores[type] = newStore;
                return newStore;
            }

            return (ComponentStore<T>)store;
        }

        /// <summary>
        /// 处理延迟销毁队列
        /// </summary>
        private void ProcessDestroyQueue()
        {
            while (entitiesToDestroy.Count > 0)
            {
                int entityId = entitiesToDestroy.Dequeue();

                if (!activeEntities.Contains(entityId))
                {
                    continue;
                }

                // 从活跃实体中移除
                activeEntities.Remove(entityId);

                // 从所有组件存储中移除
                foreach (var store in componentStores.Values)
                {
                    if (store.HasComponent(entityId))
                    {
                        store.RemoveComponent(entityId);
                    }
                }
            }
        }

        // ============================================
        // 调试和统计
        // ============================================

        /// <summary>
        /// 获取所有实体ID
        /// </summary>
        public List<int> GetAllEntities()
        {
            return new List<int>(activeEntities);
        }

        /// <summary>
        /// 获取组件统计信息
        /// </summary>
        public Dictionary<string, int> GetComponentStats()
        {
            var stats = new Dictionary<string, int>();

            foreach (var kvp in componentStores)
            {
                string typeName = kvp.Key.Name;
                int count = kvp.Value.GetComponentCount();
                stats[typeName] = count;
            }

            return stats;
        }

        /// <summary>
        /// 打印调试信息
        /// </summary>
        public void DebugPrint()
        {
            Debug.Log($"=== ECS World 状态 ===");
            Debug.Log($"实体数量: {EntityCount}");
            Debug.Log($"系统数量: {SystemCount}");
            Debug.Log($"组件类型数量: {componentStores.Count}");

            Debug.Log("\n组件统计:");
            foreach (var kvp in GetComponentStats())
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value}");
            }
        }

        /// <summary>
        /// 清空所有数据
        /// </summary>
        public void Clear()
        {
            activeEntities.Clear();
            entitiesToDestroy.Clear();

            foreach (var store in componentStores.Values)
            {
                if (store is IComponentStore componentStore)
                {
                    // 清空组件存储
                    var allEntities = GetAllEntities();
                    foreach (var entityId in allEntities)
                    {
                        componentStore.RemoveComponent(entityId);
                    }
                }
            }

            componentStores.Clear();
            nextEntityId = 1;
        }
    }
}