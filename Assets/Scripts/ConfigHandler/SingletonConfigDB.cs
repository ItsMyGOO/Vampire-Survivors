using System.Collections.Generic;

namespace ConfigHandler
{
    /// <summary>
    /// 配置数据库接口
    /// </summary>
    public interface IConfigDB<TKey, TValue>
    {
        bool TryGet(TKey key, out TValue value);
        TValue Get(TKey key);
        IEnumerable<TKey> GetAllKeys();
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

        public IEnumerable<TKey> GetAllKeys()
        {
            return _data.Keys;
        }

        protected void Add(TKey key, TValue value)
        {
            _data[key] = value;
        }
    }
}