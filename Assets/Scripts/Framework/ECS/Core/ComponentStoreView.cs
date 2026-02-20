namespace ECS.Core
{
    /// <summary>
    /// ComponentStore 的只读枚举视图，支持：
    ///   foreach (var (entityId, component) in world.GetComponents&lt;T&gt;())
    /// 无额外堆分配（struct enumerator）；迭代期间不得增删该类型组件。
    /// </summary>
    public readonly struct ComponentStoreView<T> where T : new()
    {
        private readonly ComponentStore<T> _store;

        public ComponentStoreView(ComponentStore<T> store)
        {
            _store = store;
        }

        public Enumerator GetEnumerator() => new Enumerator(_store);

        public struct Enumerator
        {
            private readonly int[] _ids;
            private readonly T[]   _data;
            private readonly int   _count;
            private int            _index;

            public Enumerator(ComponentStore<T> store)
            {
                store.GetRawArrays(out _ids, out _data, out _count);
                _index = -1;
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _count;
            }

            // 解构为 (int entityId, T component)，匹配现有 System 写法
            public (int entityId, T component) Current
                => (_ids[_index], _data[_index]);
        }
    }
}