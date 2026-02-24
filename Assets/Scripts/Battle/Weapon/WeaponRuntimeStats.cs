using System;
using ConfigHandler;

namespace Battle.Weapon
{
    /// <summary>
    /// 单个武器的运行时状态（升级 / Buff / Lua 修改）
    /// </summary>
    [Serializable]
    public class WeaponRuntimeStats
    {
        public string weaponId;
        public int level;
        
        public float cooldown;          
        public bool orbitSpawned;    
        
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
        /// 根据 cfg + 当前修正 + 玩家被动倍率，生成最终战斗数值。
        /// finalDamage = baseDamage * levelMul * playerDamageMul * weapon.damageMul
        /// interval = baseInterval * weapon.fireRateMul * playerCooldownMul
        /// speed = (baseSpeed + weapon.speedAdd) * playerProjectileSpeedMul
        /// </summary>
        public WeaponFinalStats BuildFinalStats(
            WeaponConfig cfg,
            float playerDamageMul = 1f,
            float playerCooldownMul = 1f,
            float playerProjectileSpeedMul = 1f)
        {
            var baseStats = cfg.battle.baseStats;
            var projectile = cfg.battle.projectile;
            var orbit = cfg.battle.orbit;

            var pState = projectile == null
                ? null
                : new ProjectileFinalStats
                {
                    count = baseStats.count + countAdd,
                    interval = projectile.interval * fireRateMul * playerCooldownMul,
                    speed = (projectile.speed + speedAdd) * playerProjectileSpeedMul,
                    range = projectile.range + rangeAdd
                };
            var oState = orbit == null
                ? null
                : new OrbitFinalStats
                {
                    count = baseStats.count + countAdd,
                    radius = orbit.radius + orbitRadiusAdd,
                    speed = (orbit.speed + orbitSpeedAdd) * playerProjectileSpeedMul
                };

            float baseDamage = baseStats.damage * level + damageAdd;
            return new WeaponFinalStats
            {
                damage = baseDamage * damageMul * playerDamageMul,
                knockback = baseStats.knockback + knockbackAdd,
                projectile = pState,
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
