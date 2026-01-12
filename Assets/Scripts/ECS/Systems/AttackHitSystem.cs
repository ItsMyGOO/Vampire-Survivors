using System.Collections.Generic;
using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    public class AttackHitSystem : SystemBase
    {
        private const float KNOCKBACK_DURATION = 0.15f;

        public override void Update(World world, float deltaTime)
        {
            var projectiles = new List<(
                int entity,
                PositionComponent pos,
                ColliderComponent col,
                DamageSourceComponent dmg,
                VelocityComponent vel
                )>();

            foreach (var (entity, damageSource) in world.GetComponents<DamageSourceComponent>())
            {
                if (world.HasComponent<PositionComponent>(entity) &&
                    world.HasComponent<ColliderComponent>(entity))
                {
                    var pos = world.GetComponent<PositionComponent>(entity);
                    var col = world.GetComponent<ColliderComponent>(entity);
                    var vel = world.GetComponent<VelocityComponent>(entity);

                    projectiles.Add((entity, pos, col, damageSource, vel));
                }
            }

            foreach (var (enemyId, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<PositionComponent>(enemyId) ||
                    !world.HasComponent<ColliderComponent>(enemyId))
                    continue;

                var enemyPos = world.GetComponent<PositionComponent>(enemyId);
                var enemyCol = world.GetComponent<ColliderComponent>(enemyId);

                foreach (var proj in projectiles)
                {
                    if (CheckCollision(proj.pos, proj.col, enemyPos, enemyCol))
                    {
                        ApplyHit(world, proj, enemyId, enemyPos);
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

        private void ApplyHit(World world,
            (int entity, PositionComponent pos, ColliderComponent col, DamageSourceComponent dmg, VelocityComponent vel)
                proj,
            int enemyId, PositionComponent enemyPos)
        {
            // 1. 应用伤害
            if (world.HasComponent<HealthComponent>(enemyId))
            {
                var health = world.GetComponent<HealthComponent>(enemyId);
                health.current -= proj.dmg.damage;
            }

            // 2. 添加击退组件（如果武器有击退值）
            if (proj.dmg.knockBack > 0.001f)
            {
                if (!world.HasComponent<KnockBackComponent>(enemyId))
                {
                    Vector2 knockBackDir = CalculateKnockBackDirection(proj.pos, proj.vel, enemyPos);

                    world.AddComponent(enemyId, new KnockBackComponent()
                    {
                        forceX = knockBackDir.x * proj.dmg.knockBack,
                        forceY = knockBackDir.y * proj.dmg.knockBack,
                        time = KNOCKBACK_DURATION
                    });
                }
                else
                {
                    var knockBack = world.GetComponent<KnockBackComponent>(enemyId);
                    Vector2 knockBackDir = CalculateKnockBackDirection(proj.pos, proj.vel, enemyPos);

                    knockBack.forceX += knockBackDir.x * proj.dmg.knockBack;
                    knockBack.forceY += knockBackDir.y * proj.dmg.knockBack;
                    knockBack.time = KNOCKBACK_DURATION;
                }
            }
            
            // 添加hit动画
            world.AddComponent(enemyId, new AnimationCommandComponent()
            {
                command = "play",
                anim_name = "Hit"
            });

            // 3. 处理投射物穿透
            if (world.HasComponent<ProjectileComponent>(proj.entity))
            {
                var projectile = world.GetComponent<ProjectileComponent>(proj.entity);
                projectile.hit_count++;

                if (projectile.hit_count >= projectile.pierce)
                {
                    world.DestroyEntity(proj.entity);
                }
            }
        }

        private Vector2 CalculateKnockBackDirection(PositionComponent projPos, VelocityComponent projVel,
            PositionComponent enemyPos)
        {
            if (projVel != null)
            {
                float velMagnitude = Mathf.Sqrt(projVel.x * projVel.x + projVel.y * projVel.y);

                if (velMagnitude > 0.001f)
                {
                    return new Vector2(projVel.x / velMagnitude, projVel.y / velMagnitude);
                }
            }

            float dx = enemyPos.x - projPos.x;
            float dy = enemyPos.y - projPos.y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);

            if (dist > 0.001f)
            {
                return new Vector2(dx / dist, dy / dist);
            }

            return Vector2.right;
        }
    }
}