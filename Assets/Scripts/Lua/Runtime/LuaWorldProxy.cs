using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Lua
{
    public sealed class LuaWorldProxy
    {
        // ===== Lua references（全部缓存）=====
        private LuaTable _world;
        private LuaTable _components;

        // Component keys（来自 ComponentRegistry）
        private LuaTable _positionComp;
        private LuaTable _velocityComp;
        private LuaTable _animStateComp;

        // Bucket cache：ComponentType -> Bucket
        private readonly Dictionary<LuaTable, LuaTable> _bucketCache
            = new Dictionary<LuaTable, LuaTable>();

        // ===== 构造 =====
        public LuaWorldProxy(LuaTable world, LuaTable registry)
        {
            _world = world;
            _components = _world.Get<LuaTable>("components");

            CacheComponentTypes(registry);
        }

        // ===== ComponentRegistry 缓存 =====
        private void CacheComponentTypes(LuaTable registry)
        {
            _positionComp = registry.Get<LuaTable>("Position");
            _velocityComp = registry.Get<LuaTable>("Velocity");
            _animStateComp = registry.Get<LuaTable>("AnimState");

            registry.Dispose();
        }

        // ===== Bucket 访问（带缓存）=====
        private LuaTable GetBucket(LuaTable compType)
        {
            if (!_bucketCache.TryGetValue(compType, out var bucket))
            {
                bucket = _components.Get<LuaTable, LuaTable>(compType);
                _bucketCache[compType] = bucket;
            }

            return bucket;
        }

        // =================================================
        // ================ 对外只读 API ===================
        // =================================================

        public bool TryGetPosition(int eid, out Vector3 pos)
        {
            var bucket = GetBucket(_positionComp);
            if (bucket == null || !TryGetValue(bucket, eid, out LuaTable p))
            {
                pos = default;
                return false;
            }

            pos = new Vector3(
                p.Get<float>("x"),
                p.Get<float>("y"),
                p.Get<float>("z")
            );
            return true;
        }
        //
        // public bool TryGetVelocity(int eid, out Vector3 vel)
        // {
        //     var bucket = GetBucket(_velocityComp);
        //     if (bucket == null || !bucket.TryGetValue(eid, out LuaTable v))
        //     {
        //         vel = default;
        //         return false;
        //     }
        //
        //     vel = new Vector3(
        //         v.Get<float>("x"),
        //         v.Get<float>("y"),
        //         v.Get<float>("z")
        //     );
        //     return true;
        // }
        //
        // public string GetAnimState(int eid)
        // {
        //     var bucket = GetBucket(_animStateComp);
        //     return bucket != null && bucket.TryGetValue(eid, out var state)
        //         ? state as string
        //         : null;
        // }

        // ===== 释放（切 World / 退游戏）=====
        public void Dispose()
        {
            foreach (var kv in _bucketCache)
                kv.Value?.Dispose();

            _bucketCache.Clear();

            _world?.Dispose();
            _components?.Dispose();

            _positionComp?.Dispose();
            _velocityComp?.Dispose();
            _animStateComp?.Dispose();
        }

        private bool TryGetValue(LuaTable bucket, int eid, out LuaTable value)
        {
            if (bucket == null)
            {
                value = null;
                return false;
            }

            value = bucket.Get<int, LuaTable>(eid);
            return value != null;
        }
    }
}