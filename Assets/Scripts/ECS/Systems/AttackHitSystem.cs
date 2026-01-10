using System.Collections.Generic;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 攻击命中系统
    /// 职责: 检测投射物与敌人的碰撞，应用伤害
    /// </summary>
    public class AttackHitSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            // 收集所有投射物
            var projectiles =
                new List<(int entity, PositionComponent pos, ColliderComponent col, DamageSourceComponent dmg)>();

            foreach (var (entity, damageSource) in world.GetComponents<DamageSourceComponent>())
            {
                if (world.HasComponent<PositionComponent>(entity) &&
                    world.HasComponent<ColliderComponent>(entity))
                {
                    var pos = world.GetComponent<PositionComponent>(entity);
                    var col = world.GetComponent<ColliderComponent>(entity);
                    projectiles.Add((entity, pos, col, damageSource));
                }
            }

            // 遍历所有敌人
            foreach (var (enemyId, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<PositionComponent>(enemyId) ||
                    !world.HasComponent<ColliderComponent>(enemyId))
                {
                    continue;
                }

                var enemyPos = world.GetComponent<PositionComponent>(enemyId);
                var enemyCol = world.GetComponent<ColliderComponent>(enemyId);

                // 检查与所有投射物的碰撞
                foreach (var proj in projectiles)
                {
                    if (CheckCollision(proj.pos, proj.col, enemyPos, enemyCol))
                    {
                        ApplyDamage(world, proj.entity, enemyId, proj.dmg.damage);
                    }
                }
            }
        }

        private bool CheckCollision(PositionComponent pos1, ColliderComponent col1,
            PositionComponent pos2, ColliderComponent col2)
        {
            float dx = pos1.x - pos2.x;
            float dy = pos1.y - pos2.y;
            float distSq = dx * dx + dy * dy;
            float radiusSum = col1.radius + col2.radius;

            return distSq <= radiusSum * radiusSum;
        }

        private void ApplyDamage(World world, int projectileId, int enemyId, float damage)
        {
            // 应用伤害
            if (world.HasComponent<HealthComponent>(enemyId))
            {
                var health = world.GetComponent<HealthComponent>(enemyId);
                health.current -= damage;
            }

            // 处理穿透
            if (world.HasComponent<ProjectileComponent>(projectileId))
            {
                var projectile = world.GetComponent<ProjectileComponent>(projectileId);
                projectile.hit_count++;

                if (projectile.hit_count >= projectile.pierce)
                {
                    world.DestroyEntity(projectileId);
                }
            }
        }
    }
}