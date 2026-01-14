using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    /// <summary>
    /// 武器发射系统（重构版）
    /// 职责：
    /// 1. 管理武器冷却
    /// 2. 决定是否触发发射
    /// 3. 调用 SpawnService 生成实体
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

                foreach (var weapon in slots.weapons)
                {
                    if (!WeaponConfigDB.Instance.TryGet(weapon.weapon_type, out var def))
                        continue;

                    switch (def.battle.Type)
                    {
                        case WeaponType.Projectile:
                            UpdateProjectileWeapon(world, owner, ownerPos, weapon, def, deltaTime);
                            break;

                        case WeaponType.Orbit:
                            UpdateOrbitWeapon(world, owner, weapon, def);
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
            float deltaTime)
        {
            weapon.cooldown -= deltaTime;
            if (weapon.cooldown > 0f)
                return;

            var def = cfg.battle;
            weapon.cooldown = def.projectile.interval;

            int target = FindNearestEnemy(world, ownerPos);
            Vector2 dir = CalculateDirection(world, ownerPos, target);

            int count = Mathf.RoundToInt(def.baseStats.count * weapon.level);
            for (int i = 0; i < count; i++)
            {
                ProjectileSpawnService.Spawn(
                    world,
                    owner,
                    ownerPos,
                    dir,
                    cfg,
                    weapon.level
                );
            }
        }

        // ===================== Orbit =====================

        private void UpdateOrbitWeapon(
            World world,
            int owner,
            WeaponSlotsComponent.WeaponData weapon,
            WeaponConfig cfg)
        {
            if (weapon.orbitSpawned)
                return;

            weapon.orbitSpawned = true;

            var def = cfg.battle;
            int count = Mathf.RoundToInt(def.baseStats.count * weapon.level);
            float step = Mathf.PI * 2f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = step * i;

                OrbitSpawnService.Spawn(
                    world,
                    owner,
                    angle,
                    cfg,
                    weapon.level
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
