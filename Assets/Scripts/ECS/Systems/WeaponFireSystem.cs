using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    /// <summary>
    /// 武器发射系统（重构版 + 运行时属性支持）
    /// 职责：
    /// 1. 管理武器冷却
    /// 2. 决定是否触发发射
    /// 3. 调用 SpawnService 生成实体（使用运行时属性）
    /// </summary>
    public class WeaponFireSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (owner, slots) in world.GetComponents<WeaponSlotsComponent>())
            {
                if (!world.HasComponent<PositionComponent>(owner))
                    continue;

                var ownerPos = world.GetComponent<PositionComponent>(owner);
                
                // 获取运行时属性组件
                world.TryGetComponent<WeaponRuntimeStatsComponent>(owner, out var runtimeStats);

                foreach (var weapon in slots.weapons)
                {
                    if (!WeaponConfigDB.Instance.TryGet(weapon.weapon_type, out var def))
                        continue;

                    // 获取武器的运行时属性
                    WeaponRuntimeStatsComponent.WeaponStats stats = null;
                    if (runtimeStats != null)
                    {
                        runtimeStats.TryGetStats(weapon.weapon_type, out stats);
                    }

                    switch (def.battle.Type)
                    {
                        case WeaponType.Projectile:
                            UpdateProjectileWeapon(world, owner, ownerPos, weapon, def, stats, deltaTime);
                            break;

                        case WeaponType.Orbit:
                            UpdateOrbitWeapon(world, owner, weapon, def, stats);
                            break;
                    }
                }
            }
        }

        // ===================== Projectile =====================

        private void UpdateProjectileWeapon(
            World world,
            int owner,
            PositionComponent ownerPos,
            WeaponSlotsComponent.WeaponData weapon,
            WeaponConfig cfg,
            WeaponRuntimeStatsComponent.WeaponStats stats,
            float deltaTime)
        {
            weapon.cooldown -= deltaTime;
            if (weapon.cooldown > 0f)
                return;

            // 使用运行时属性计算冷却时间
            float fireRate = stats != null ? stats.GetFinalFireRate() : cfg.battle.projectile.interval;
            weapon.cooldown = fireRate;

            int target = FindNearestEnemy(world, ownerPos);
            Vector2 baseDir = CalculateDirection(world, ownerPos, target);

            // 使用运行时属性计算投射物数量
            int count = stats != null ? stats.GetFinalProjectileCount() : cfg.battle.baseStats.count;
            
            for (int i = 0; i < count; i++)
            {
                
                Vector2 dir = ProjectileSpawnService.Calculate(
                    baseDir, i, count, 5);
                ProjectileSpawnService.Spawn(
                    world,
                    owner,
                    ownerPos,
                    dir,
                    cfg,
                    weapon.level,
                    stats  // 传递运行时属性
                );
            }
        }

        // ===================== Orbit =====================

        private void UpdateOrbitWeapon(
            World world,
            int owner,
            WeaponSlotsComponent.WeaponData weapon,
            WeaponConfig cfg,
            WeaponRuntimeStatsComponent.WeaponStats stats)
        {
            if (weapon.orbitSpawned)
                return;

            weapon.orbitSpawned = true;

            // 使用运行时属性计算轨道武器数量
            int count = stats != null ? stats.GetFinalOrbitCount() : cfg.battle.baseStats.count;
            float step = Mathf.PI * 2f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = step * i;

                OrbitSpawnService.Spawn(
                    world,
                    owner,
                    angle,
                    cfg,
                    weapon.level,
                    stats  // 传递运行时属性
                );
            }
        }

        // ===================== Target =====================

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

        private Vector2 CalculateDirection(
            World world,
            PositionComponent ownerPos,
            int target)
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
