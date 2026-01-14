using System.Collections.Generic;

namespace Framework.Config
{
    /// <summary>
    /// 配置数据库接口
    /// </summary>
    public interface IConfigDB<TKey, TValue>
    {
        bool TryGet(TKey key, out TValue value);
        TValue Get(TKey key);
        public IEnumerable<TKey> AllKeys { get; }

        public IEnumerable<TValue> AllValues { get; }
    }

    /// <summary>
    /// 单例配置数据库基类
    /// </summary>
    public abstract class SingletonConfigDB<TDB, TKey, TValue> : IConfigDB<TKey, TValue>
        where TDB : SingletonConfigDB<TDB, TKey, TValue>
    {
        private static TDB _instance;
        public static TDB Instance => _instance;

        protected readonly Dictionary<TKey, TValue> _data = new Dictionary<TKey, TValue>();
        public Dictionary<TKey, TValue> Data => _data;

        public static void Initialize(TDB db)
        {
            _instance = db;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            return _data.TryGetValue(key, out value);
        }

        public TValue Get(TKey key)
        {
            _data.TryGetValue(key, out var value);
            return value;
        }

        public IEnumerable<TKey> AllKeys => _data.Keys;

        public IEnumerable<TValue> AllValues => _data.Values;

        protected void Add(TKey key, TValue value)
        {
            _data[key] = value;
        }
    }
}