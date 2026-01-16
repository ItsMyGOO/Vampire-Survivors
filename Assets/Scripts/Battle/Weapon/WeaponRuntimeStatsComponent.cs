using System;
using System.Collections.Generic;
using ConfigHandler;

namespace Battle.Weapon
{
    /// <summary>
    /// 挂在 Player / WeaponOwner 上
    /// 管理所有武器的 RuntimeStats
    /// </summary>
    [Serializable]
    public class WeaponRuntimeStatsComponent
    {
        private readonly Dictionary<string, WeaponRuntimeStats> _stats
            = new Dictionary<string, WeaponRuntimeStats>();
        
        public IEnumerable<WeaponRuntimeStats> GetAllWeapons()
        {
            return _stats.Values;
        }

        public WeaponRuntimeStats GetOrCreate(string weaponId)
        {
            if (!_stats.TryGetValue(weaponId, out var stats))
            {
                stats = new WeaponRuntimeStats
                {
                    weaponId = weaponId,
                    level = 1,
                    cooldown = 0f,
                    orbitSpawned = false
                };
                _stats.Add(weaponId, stats);
            }
            return stats;
        }
        
        public WeaponRuntimeStats AddWeapon(string weaponId, int initialLevel = 1)
        {
            if (_stats.ContainsKey(weaponId))
            {
                return _stats[weaponId];
            }

            var stats = new WeaponRuntimeStats
            {
                weaponId = weaponId,
                level = initialLevel,
                cooldown = 0f,
                orbitSpawned = false
            };
            _stats.Add(weaponId, stats);
            return stats;
        }
        
        public bool HasWeapon(string weaponId)
        {
            return _stats.ContainsKey(weaponId);
        }
        
        public WeaponFinalStats BuildFinalStats(
            string weaponId,
            WeaponConfig cfg)
        {
            var runtime = GetOrCreate(weaponId);
            return runtime.BuildFinalStats(cfg);
        }
        
        public bool TryGetRuntime(string weaponId, out WeaponRuntimeStats stats)
        {
            return _stats.TryGetValue(weaponId, out stats);
        }
        
        public bool RemoveWeapon(string weaponId)
        {
            return _stats.Remove(weaponId);
        }
        
        public int WeaponCount => _stats.Count;
    }
}
