using System.Collections.Generic;
using UnityEngine;

namespace ECS.Core
{
    public interface IComponentStore
    {
        bool HasComponent(int entityId);
        void RemoveComponent(int entityId);
        int GetComponentCount();
    }

// ============================================
// 泛型组件存储
// ============================================

    public class ComponentStore<T> : IComponentStore where T : class, new()
    {
        private Dictionary<int, T> components = new Dictionary<int, T>();

        /// <summary>
        /// 添加组件
        /// </summary>
        public void Add(int entityId, T component)
        {
            components[entityId] = component;
        }

        /// <summary>
        /// 获取组件（直接返回引用，可修改）
        /// </summary>
        public T Get(int entityId)
        {
            if (components.TryGetValue(entityId, out T component))
            {
                return component;
            }

            return null;
        }

        /// <summary>
        /// 检查是否有组件
        /// </summary>
        public bool HasComponent(int entityId)
        {
            return components.ContainsKey(entityId);
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public void Remove(int entityId)
        {
            components.Remove(entityId);
        }

        /// <summary>
        /// 获取所有组件（用于遍历）
        /// </summary>
        public IEnumerable<KeyValuePair<int, T>> GetAll()
        {
            return components;
        }

        /// <summary>
        /// 组件数量
        /// </summary>
        public int Count => components.Count;

        /// <summary>
        /// 清空所有组件
        /// </summary>
        public void Clear()
        {
            components.Clear();
        }

        // IComponentStore 接口实现
        void IComponentStore.RemoveComponent(int entityId)
        {
            Remove(entityId);
        }

        int IComponentStore.GetComponentCount()
        {
            return Count;
        }
    }
}