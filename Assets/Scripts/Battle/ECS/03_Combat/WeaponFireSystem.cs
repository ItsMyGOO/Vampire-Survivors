using Battle.Weapon;
using ConfigHandler;
using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    public class WeaponFireSystem : SystemBase
    {
        /// <summary>
        /// 目标搜索半径，与 AIMovementSystem.AI_ACTIVE_RADIUS 对齐。
        /// 范围内无敌人时自动降级为全量扫描兜底。
        /// </summary>
        private const float TARGET_SEARCH_RADIUS = 30f;

        // 预分配：QueryEnemies 结果写入此 buffer，零 GC
        private readonly int[] _targetBuffer = new int[256];

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
                float pDamageMul   = playerAttr?.damageMul ?? 1f;
                float pCooldownMul = playerAttr?.cooldownMul ?? 1f;
                float pSpeedMul    = playerAttr?.projectileSpeedMul ?? 1f;

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

                ProjectileSpawnService.Spawn(world, owner, ownerPos, dir, stats, cfg);
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
            if (weapon.orbitSpawned) return;
            weapon.orbitSpawned = true;

            float step = Mathf.PI * 2f / stats.orbit.count;

            for (int i = 0; i < stats.orbit.count; i++)
                OrbitSpawnService.Spawn(world, owner, step * i, stats, cfg);
        }

        // ================= Target =================

        private int FindNearestEnemy(World world, PositionComponent playerPos)
        {
            // 优先用共享空间索引查询范围内候选，O(k) 而非 O(N)
            if (world.TryGetService<IEnemySpatialIndex>(out var enemyIndex))
            {
                int candidateCount = enemyIndex.QueryEnemies(
                    playerPos.x, playerPos.y, TARGET_SEARCH_RADIUS, _targetBuffer);

                if (candidateCount > 0)
                    return FindNearestInBuffer(world, playerPos, _targetBuffer, candidateCount);
            }

            // 降级兜底：空间索引未就绪或范围内无敌人，全量扫描
            return FindNearestFallback(world, playerPos);
        }

        private int FindNearestInBuffer(World world, PositionComponent playerPos, int[] buffer, int count)
        {
            int nearest = -1;
            float minDistSq = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                int entity = buffer[i];
                if (!world.HasComponent<PositionComponent>(entity)) continue;

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

        /// <summary>
        /// 全量扫描兜底：空间索引未就绪或搜索半径内无敌人时使用。
        /// </summary>
        private int FindNearestFallback(World world, PositionComponent playerPos)
        {
            int nearest = -1;
            float minDistSq = float.MaxValue;

            foreach (var (entity, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity)) continue;

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

            if (!world.HasComponent<PositionComponent>(target))
                return Vector2.right;

            var targetPos = world.GetComponent<PositionComponent>(target);
            Vector2 dir = new Vector2(targetPos.x - ownerPos.x, targetPos.y - ownerPos.y);

            return dir.sqrMagnitude < 0.0001f ? Vector2.right : dir.normalized;
        }
    }
}