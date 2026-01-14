using ECS.Core;
using ConfigHandler;
using ECS;
using Game.Battle;

namespace Battle
{
    public static class PlayerWeaponInitializer
    {
        public static void Initialize(World world, int playerId)
        {
            if (!world.TryGetComponent(playerId, out WeaponSlotsComponent slots))
                return;

            var runtimeStats = new WeaponRuntimeStatsComponent();

            foreach (var w in slots.weapons)
            {
                if (!WeaponConfigDB.Instance.Data.TryGetValue(w.weapon_type, out var cfg))
                    continue;

                var stats = runtimeStats.GetOrCreateStats(w.weapon_type);
                var battle = cfg.battle;

                stats.level = w.level;
                stats.damage = battle.baseStats.damage;
                stats.projectileCount = battle.baseStats.count;
                stats.knockback = battle.baseStats.knockback;

                if (battle.Type == WeaponType.Projectile)
                {
                    stats.fireRate = battle.projectile.interval;
                    stats.projectileSpeed = battle.projectile.speed;
                    stats.projectileRange = battle.projectile.range;
                }
                else
                {
                    stats.orbitRadius = battle.orbit.radius;
                    stats.orbitSpeed = battle.orbit.speed;
                    stats.orbitCount = battle.baseStats.count;
                }
            }

            world.AddComponent(playerId, runtimeStats);
        }
    }
}