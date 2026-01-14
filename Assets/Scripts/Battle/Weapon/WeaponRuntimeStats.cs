using System;
using ConfigHandler;

namespace ECS
{
    /// <summary>
    /// 单个武器的运行时状态（升级 / Buff / Lua 修改）
    /// </summary>
    [Serializable]
    public class WeaponRuntimeStats
    {
        public string weaponId;
        public int level;

        // ========= 累计修正 =========

        public float damageAdd;
        public float damageMul = 1f;

        public int countAdd;

        public float fireRateMul = 1f;

        public float speedAdd;
        public float rangeAdd;

        public float knockbackAdd;

        public float orbitRadiusAdd;
        public float orbitSpeedAdd;

        /// <summary>
        /// 根据 cfg + 当前修正，生成最终战斗数值
        /// </summary>
        public WeaponFinalStats BuildFinalStats(WeaponConfig cfg)
        {
            var baseStats = cfg.battle.baseStats;
            var projectile = cfg.battle.projectile;
            var orbit = cfg.battle.orbit;

            var pState = projectile == null
                ? null
                : new ProjectileFinalStats()
                {
                    count = baseStats.count + countAdd,
                    interval = projectile.interval * fireRateMul,
                    speed = projectile.speed + speedAdd,
                    range = projectile.range + rangeAdd
                };
            var oState = orbit == null
                ? null
                : new OrbitFinalStats()
                {
                    count = baseStats.count + countAdd,
                    radius = orbit.radius + orbitRadiusAdd,
                    speed = orbit.speed + orbitSpeedAdd
                };

            return new WeaponFinalStats
            {
                // ===== 通用 =====
                damage = (baseStats.damage * level + damageAdd) * damageMul,
                knockback = baseStats.knockback + knockbackAdd,

                // ===== 投射物 =====
                projectile = pState,

                // ===== 环绕 =====
                orbit = oState
            };
        }

        /// <summary>
        /// 清空所有升级 / buff（例如复活、重置）
        /// </summary>
        public void ResetModifiers()
        {
            damageAdd = 0;
            damageMul = 1f;
            countAdd = 0;
            fireRateMul = 1f;
            speedAdd = 0;
            rangeAdd = 0;
            knockbackAdd = 0;
            orbitRadiusAdd = 0;
            orbitSpeedAdd = 0;
        }
    }
}