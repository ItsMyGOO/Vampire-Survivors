using System.Collections.Generic;

namespace ECS.Core
{
    public interface IComponentStore
    {
        bool HasComponent(int entityId);
        void RemoveComponent(int entityId);
        int GetComponentCount();
        void FillEntityBuffer(List<int> buffer);
    }

    /// <summary>
    /// 紧凑双数组存储：ids[] + data[] 保证值类型组件的连续内存布局，对 struct 组件 cache 友好。
    /// Dictionary 仅用作 entityId->index 的 O(1) 索引映射，不存储组件本体。
    /// 同时兼容 class 组件（T : new() 无 class 限制）。
    /// </summary>
    public class ComponentStore<T> : IComponentStore where T : new()
    {
        private const int InitialCapacity = 64;

        // 紧凑数据数组 —— struct 组件在此连续排列
        private int[] _ids   = new int[InitialCapacity];
        private T[]   _data  = new T[InitialCapacity];
        private int   _count;

        // entityId -> 数组下标，O(1) 随机访问
        private readonly Dictionary<int, int> _indexMap = new Dictionary<int, int>(InitialCapacity);

        // ============================================
        // 写操作
        // ============================================

        public void Add(int entityId, T component)
        {
            if (_indexMap.TryGetValue(entityId, out int existing))
            {
                _data[existing] = component;
                return;
            }

            EnsureCapacity(_count + 1);

            int idx = _count++;
            _ids[idx]  = entityId;
            _data[idx] = component;
            _indexMap[entityId] = idx;
        }

        /// <summary>
        /// 将修改后的 struct 写回存储（struct 组件修改后必须调用；class 组件调用无副作用）。
        /// </summary>
        public void Set(int entityId, T component)
        {
            if (_indexMap.TryGetValue(entityId, out int idx))
                _data[idx] = component;
        }

        public void Remove(int entityId)
        {
            if (!_indexMap.TryGetValue(entityId, out int idx))
                return;

            // swap-back：将末尾元素填入被删除位置，保持数组紧凑
            int last = _count - 1;
            if (idx != last)
            {
                int lastId    = _ids[last];
                _ids[idx]     = lastId;
                _data[idx]    = _data[last];
                _indexMap[lastId] = idx;
            }

            // 清理末尾
            _ids[last]  = 0;
            _data[last] = default;
            _count--;
            _indexMap.Remove(entityId);
        }

        // ============================================
        // 读操作
        // ============================================

        public T Get(int entityId)
        {
            if (_indexMap.TryGetValue(entityId, out int idx))
                return _data[idx];
            return default;
        }

        public bool HasComponent(int entityId) => _indexMap.ContainsKey(entityId);

        /// <summary>
        /// 暴露紧凑数组供 System 直接按索引遍历（零分配）。
        /// 迭代期间不得增删组件。
        /// </summary>
        public void GetRawArrays(out int[] ids, out T[] data, out int count)
        {
            ids   = _ids;
            data  = _data;
            count = _count;
        }

        public int Count => _count;

        public void Clear()
        {
            // 清空值，避免 class 组件引用泄漏
            for (int i = 0; i < _count; i++)
            {
                _ids[i]  = 0;
                _data[i] = default;
            }
            _count = 0;
            _indexMap.Clear();
        }

        // ============================================
        // IComponentStore 显式实现
        // ============================================

        void IComponentStore.RemoveComponent(int entityId) => Remove(entityId);

        int IComponentStore.GetComponentCount() => _count;

        void IComponentStore.FillEntityBuffer(List<int> buffer)
        {
            buffer.Clear();
            for (int i = 0; i < _count; i++)
                buffer.Add(_ids[i]);
        }

        // ============================================
        // 内部工具
        // ============================================

        private void EnsureCapacity(int required)
        {
            if (required <= _ids.Length) return;

            int newCap = _ids.Length * 2;
            if (newCap < required) newCap = required;

            var newIds  = new int[newCap];
            var newData = new T[newCap];

            System.Array.Copy(_ids,  newIds,  _count);
            System.Array.Copy(_data, newData, _count);

            _ids  = newIds;
            _data = newData;
        }
    }
}