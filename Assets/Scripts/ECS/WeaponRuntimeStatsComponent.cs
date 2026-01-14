using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// 武器运行时属性组件 - 存储每个武器的实际运行时属性
    /// </summary>
    [Serializable]
    public class WeaponRuntimeStatsComponent
    {
        [Serializable]
        public class WeaponStats
        {
            public string weaponId;
            public int level;
            
            // 基础属性
            public float damage;
            public int projectileCount;
            public float knockback;
            
            // 投射物专属
            public float fireRate;          // 发射间隔
            public float projectileSpeed;   // 投射物速度
            public float projectileRange;   // 射程
            
            // 环绕武器专属
            public float orbitRadius;       // 环绕半径
            public float orbitSpeed;        // 环绕速度
            public int orbitCount;          // 环绕数量
            
            // 升级累积的修改
            public float damageAdd;         // 伤害增加值
            public float damageMultiply;    // 伤害倍率
            public int countAdd;            // 数量增加
            public float fireRateMultiply;  // 射速倍率
            public float radiusAdd;         // 半径增加
            public float speedAdd;          // 速度增加
            public float rangeAdd;          // 射程增加
            public float knockbackAdd;      // 击退增加

            public WeaponStats()
            {
                damageMultiply = 1.0f;
                fireRateMultiply = 1.0f;
            }

            /// <summary>
            /// 获取最终伤害值
            /// </summary>
            public float GetFinalDamage()
            {
                return (damage + damageAdd) * damageMultiply;
            }

            /// <summary>
            /// 获取最终投射物数量
            /// </summary>
            public int GetFinalProjectileCount()
            {
                return projectileCount + countAdd;
            }

            /// <summary>
            /// 获取最终发射间隔
            /// </summary>
            public float GetFinalFireRate()
            {
                return fireRate * fireRateMultiply;
            }

            /// <summary>
            /// 获取最终环绕半径
            /// </summary>
            public float GetFinalOrbitRadius()
            {
                return orbitRadius + radiusAdd;
            }

            /// <summary>
            /// 获取最终环绕数量
            /// </summary>
            public int GetFinalOrbitCount()
            {
                return orbitCount + countAdd;
            }
            
            /// <summary>
            /// 获取最终投射物速度
            /// </summary>
            public float GetFinalProjectileSpeed()
            {
                return projectileSpeed + speedAdd;
            }
            
            /// <summary>
            /// 获取最终投射物射程
            /// </summary>
            public float GetFinalProjectileRange()
            {
                return projectileRange + rangeAdd;
            }
            
            /// <summary>
            /// 获取最终环绕速度
            /// </summary>
            public float GetFinalOrbitSpeed()
            {
                return orbitSpeed + speedAdd;
            }
            
            /// <summary>
            /// 获取最终击退力
            /// </summary>
            public float GetFinalKnockback()
            {
                return knockback + knockbackAdd;
            }
        }

        public Dictionary<string, WeaponStats> weaponStats = new Dictionary<string, WeaponStats>();

        /// <summary>
        /// 获取或创建武器统计数据
        /// </summary>
        public WeaponStats GetOrCreateStats(string weaponId)
        {
            if (!weaponStats.TryGetValue(weaponId, out var stats))
            {
                stats = new WeaponStats { weaponId = weaponId };
                weaponStats[weaponId] = stats;
            }
            return stats;
        }

        /// <summary>
        /// 获取武器统计数据
        /// </summary>
        public bool TryGetStats(string weaponId, out WeaponStats stats)
        {
            return weaponStats.TryGetValue(weaponId, out stats);
        }
    }
}
