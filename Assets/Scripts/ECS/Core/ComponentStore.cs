using System.Collections.Generic;
using UnityEngine;

namespace ECS.Core
{
    public interface IComponentStore
    {
        bool HasComponent(int entityId);
        void RemoveComponent(int entityId);
    }

    public class ComponentStore<T> : IComponentStore where T : struct
    {
        private readonly Dictionary<int, T> _components = new Dictionary<int, T>();

        public void Add(int entityId, T component)
        {
            _components[entityId] = component;
        }

        public T Get(int entityId)
        {
            // 使用 ref 避免拷贝
            return _components.GetValueOrDefault(entityId);
        }

        public bool HasComponent(int entityId)
        {
            return _components.ContainsKey(entityId);
        }

        public void Remove(int entityId)
        {
            _components.Remove(entityId);
        }

        public IEnumerable<(int entity, T component)> GetAll()
        {
            foreach (var kvp in _components)
            {
                yield return (kvp.Key, kvp.Value);
            }
        }

        public int Count => _components.Count;

        void IComponentStore.RemoveComponent(int entityId)
        {
            Remove(entityId);
        }
    }
}