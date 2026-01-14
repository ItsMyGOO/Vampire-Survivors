using System;
using System.Collections.Generic;
using ConfigHandler;

namespace ECS
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

        /// <summary>
        /// 获取或创建运行时状态
        /// </summary>
        public WeaponRuntimeStats GetOrCreate(string weaponId)
        {
            if (!_stats.TryGetValue(weaponId, out var stats))
            {
                stats = new WeaponRuntimeStats
                {
                    weaponId = weaponId,
                    level = 1
                };
                _stats.Add(weaponId, stats);
            }
            return stats;
        }

        /// <summary>
        /// 构建最终数值（System / Spawn 用）
        /// </summary>
        public WeaponFinalStats BuildFinalStats(
            string weaponId,
            WeaponConfig cfg)
        {
            var runtime = GetOrCreate(weaponId);
            return runtime.BuildFinalStats(cfg);
        }

        /// <summary>
        /// 用于升级系统 / Lua
        /// </summary>
        public bool TryGetRuntime(string weaponId, out WeaponRuntimeStats stats)
        {
            return _stats.TryGetValue(weaponId, out stats);
        }
    }
}