using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    public class WeaponFireSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (owner, slots) in world.GetComponents<WeaponSlotsComponent>())
            {
                if (!world.HasComponent<PositionComponent>(owner))
                    continue;

                var ownerPos = world.GetComponent<PositionComponent>(owner);

                if (!world.TryGetComponent<WeaponRuntimeStatsComponent>(owner, out var runtimeStats))
                    continue;

                foreach (var weapon in slots.weapons)
                {
                    if (!WeaponConfigDB.Instance.TryGet(weapon.weapon_type, out var cfg))
                        continue;

                    var stats = runtimeStats.BuildFinalStats(weapon.weapon_type, cfg);

                    switch (cfg.battle.Type)
                    {
                        case WeaponType.Projectile:
                            UpdateProjectile(world, owner, ownerPos, weapon, stats, deltaTime, cfg);
                            break;

                        case WeaponType.Orbit:
                            UpdateOrbit(world, owner, weapon, stats, cfg);
                            break;
                    }
                }
            }
        }

        // ================= Projectile =================

        private void UpdateProjectile(
            World world,
            int owner,
            PositionComponent ownerPos,
            WeaponSlotsComponent.WeaponData weapon,
            WeaponFinalStats stats,
            float deltaTime,
            WeaponConfig cfg)
        {
            weapon.cooldown -= deltaTime;
            if (weapon.cooldown > 0f)
                return;

            weapon.cooldown = stats.projectile.interval;

            int target = FindNearestEnemy(world, ownerPos);
            Vector2 baseDir = CalculateDirection(world, ownerPos, target);

            for (int i = 0; i < stats.projectile.count; i++)
            {
                var dir = ProjectileSpawnService.Calculate(
                    baseDir, i, stats.projectile.count, 5f);

                ProjectileSpawnService.Spawn(
                    world,
                    owner,
                    ownerPos,
                    dir,
                    stats,
                    cfg
                );
            }
        }

        // ================= Orbit =================

        private void UpdateOrbit(
            World world,
            int owner,
            WeaponSlotsComponent.WeaponData weapon,
            WeaponFinalStats stats,
            WeaponConfig cfg)
        {
            if (weapon.orbitSpawned)
                return;

            weapon.orbitSpawned = true;

            float step = Mathf.PI * 2f / stats.orbit.count;

            for (int i = 0; i < stats.orbit.count; i++)
            {
                OrbitSpawnService.Spawn(
                    world,
                    owner,
                    step * i,
                    stats,
                    cfg
                );
            }
        }

        // ===== Target =====（不变）
        private int FindNearestEnemy(World world, PositionComponent playerPos)
        {
            /* 原样 */
            return -1;
        }

        private Vector2 CalculateDirection(World world, PositionComponent ownerPos, int target)
        {
            /* 原样 */
            return Vector2.right;
        }
    }
}