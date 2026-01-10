using System;
using System.Collections.Generic;

namespace ECS.Core
{
    public class World
    {
        private int _nextEntityId = 1;
        private readonly Dictionary<Type, object> _componentStores = new Dictionary<Type, object>();
        private readonly List<ISystem> _systems = new List<ISystem>();
        private readonly HashSet<int> _activeEntities = new HashSet<int>();
        private readonly Queue<int> entitiesToDestroy = new Queue<int>();

        // 创建实体
        public int CreateEntity()
        {
            int id = _nextEntityId++;
            _activeEntities.Add(id);
            return id;
        }

        // 销毁实体（延迟销毁）
        public void DestroyEntity(int entityId)
        {
            entitiesToDestroy.Enqueue(entityId);
        }

        // 添加组件
        public void AddComponent<T>(int entityId, T component) where T : struct
        {
            var store = GetOrCreateStore<T>();
            store.Add(entityId, component);
        }

        // 获取组件引用
        public T GetComponent<T>(int entityId) where T : struct
        {
            var store = GetOrCreateStore<T>();
            return store.Get(entityId);
        }

        // 检查是否有组件
        public bool HasComponent<T>(int entityId) where T : struct
        {
            if (!_componentStores.TryGetValue(typeof(T), out var obj))
            {
                return false;
            }

            var store = (ComponentStore<T>)obj;
            return store.HasComponent(entityId);
        }

        // 移除组件
        public void RemoveComponent<T>(int entityId) where T : struct
        {
            IComponentStore store = GetOrCreateStore<T>();
            store.RemoveComponent(entityId);
        }

        // 获取所有拥有某组件的实体
        public IEnumerable<(int entity, T component)> GetComponents<T>() where T : struct
        {
            var store = GetOrCreateStore<T>();
            return store.GetAll();
        }

        // 注册系统
        public void RegisterSystem(ISystem system)
        {
            _systems.Add(system);
        }

        // 更新所有系统
        public void Update(float deltaTime)
        {
            // 更新系统
            foreach (var system in _systems)
            {
                system.Update(this, deltaTime);
            }

            // 处理延迟销毁
            ProcessDestroyQueue();
        }

        // 内部方法
        private ComponentStore<T> GetOrCreateStore<T>() where T : struct
        {
            var type = typeof(T);
            if (!_componentStores.TryGetValue(type, out var obj))
            {
                var store = new ComponentStore<T>();
                _componentStores[type] = store;
                return store;
            }

            return (ComponentStore<T>)obj;
        }

        private void ProcessDestroyQueue()
        {
            while (entitiesToDestroy.Count > 0)
            {
                int entityId = entitiesToDestroy.Dequeue();
                _activeEntities.Remove(entityId);

                // 从所有组件存储中移除
                foreach (var store in _componentStores.Values)
                {
                    ((IComponentStore)store).RemoveComponent(entityId);
                }
            }
        }
    }
}