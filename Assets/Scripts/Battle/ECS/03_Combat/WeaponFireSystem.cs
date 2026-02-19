using Battle.Player;
using Battle.Weapon;
using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    public class WeaponFireSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (owner, weaponStats) in world.GetComponents<WeaponRuntimeStatsComponent>())
            {
                if (!world.HasComponent<PositionComponent>(owner))
                    continue;

                var ownerPos = world.GetComponent<PositionComponent>(owner);

                var playerAttr = world.HasComponent<PlayerAttributeComponent>(owner)
                    ? world.GetComponent<PlayerAttributeComponent>(owner)
                    : null;
                float pDamageMul = playerAttr?.damageMul ?? 1f;
                float pCooldownMul = playerAttr?.cooldownMul ?? 1f;
                float pSpeedMul = playerAttr?.projectileSpeedMul ?? 1f;

                foreach (var (weaponId, weapon) in weaponStats.GetAllWeapons())
                {
                    if (!WeaponConfigDB.Instance.TryGet(weaponId, out var cfg))
                        continue;

                    var stats = weapon.BuildFinalStats(cfg, pDamageMul, pCooldownMul, pSpeedMul);

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
            WeaponRuntimeStats weapon,
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
            WeaponRuntimeStats weapon,
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

        // ===== Target =====
        private int FindNearestEnemy(World world, PositionComponent playerPos)
        {
            int nearest = -1;
            float minDistSq = float.MaxValue;

            foreach (var (entity, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity))
                    continue;

                var pos = world.GetComponent<PositionComponent>(entity);
                float dx = pos.x - playerPos.x;
                float dy = pos.y - playerPos.y;
                float distSq = dx * dx + dy * dy;

                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    nearest = entity;
                }
            }

            return nearest;
        }

        private Vector2 CalculateDirection(World world, PositionComponent ownerPos, int target)
        {
            if (target == -1)
                return Vector2.right;

            var targetPos = world.GetComponent<PositionComponent>(target);
            if (targetPos == null)
                return Vector2.right;

            Vector2 dir = new Vector2(
                targetPos.x - ownerPos.x,
                targetPos.y - ownerPos.y
            );

            return dir.sqrMagnitude < 0.0001f
                ? Vector2.right
                : dir.normalized;
        }
    }
}
