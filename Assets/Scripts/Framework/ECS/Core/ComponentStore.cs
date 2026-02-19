using System.Collections.Generic;

namespace ECS.Core
{
    public interface IComponentStore
    {
        bool HasComponent(int entityId);
        void RemoveComponent(int entityId);
        int GetComponentCount();
        // 供 World 填充缓冲区用
        void FillEntityBuffer(List<int> buffer);
    }

    public class ComponentStore<T> : IComponentStore where T : class, new()
    {
        private readonly Dictionary<int, T> _components = new Dictionary<int, T>();

        // 同步维护的实体 ID 列表，避免每次遍历 Dictionary 再收集
        private readonly List<int> _entityList = new List<int>();

        public void Add(int entityId, T component)
        {
            bool isNew = !_components.ContainsKey(entityId);
            _components[entityId] = component;
            if (isNew)
                _entityList.Add(entityId);
        }

        public T Get(int entityId)
        {
            _components.TryGetValue(entityId, out T component);
            return component;
        }

        public bool HasComponent(int entityId)
        {
            return _components.ContainsKey(entityId);
        }

        public void Remove(int entityId)
        {
            if (_components.Remove(entityId))
                _entityList.Remove(entityId);
        }

        /// <summary>
        /// 获取所有组件（用于 GetComponents 遍历，直接迭代 Dictionary）
        /// 返回具体类型，避免 foreach 通过 IEnumerable 接口产生装箱
        /// </summary>
        public Dictionary<int, T> GetAllDirect()
        {
            return _components;
        }

        public int Count => _components.Count;

        public void Clear()
        {
            _components.Clear();
            _entityList.Clear();
        }

        // IComponentStore 接口实现
        void IComponentStore.RemoveComponent(int entityId) => Remove(entityId);

        int IComponentStore.GetComponentCount() => Count;

        /// <summary>
        /// 将当前实体 ID 列表复制到外部缓冲区（零 GC）
        /// </summary>
        void IComponentStore.FillEntityBuffer(List<int> buffer)
        {
            buffer.Clear();
            buffer.AddRange(_entityList);
        }
    }
}