using System.Collections.Generic;
using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    public class AttackHitSystem : SystemBase
    {
        private const float KNOCKBACK_DURATION = 0.15f;

        // 复用列表避免热路径分配
        private readonly List<(int entity, PositionComponent pos, ColliderComponent col,
            DamageSourceComponent dmg, VelocityComponent vel, bool hasVel)> _projectiles
            = new List<(int, PositionComponent, ColliderComponent, DamageSourceComponent, VelocityComponent, bool)>();

        public override void Update(World world, float deltaTime)
        {
            _projectiles.Clear();

            foreach (var (entity, damageSource) in world.GetComponents<DamageSourceComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity) ||
                    !world.HasComponent<ColliderComponent>(entity))
                    continue;

                var pos = world.GetComponent<PositionComponent>(entity);
                var col = world.GetComponent<ColliderComponent>(entity);
                bool hasVel = world.HasComponent<VelocityComponent>(entity);
                var vel = hasVel ? world.GetComponent<VelocityComponent>(entity) : default;

                _projectiles.Add((entity, pos, col, damageSource, vel, hasVel));
            }

            foreach (var (enemyId, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<PositionComponent>(enemyId) ||
                    !world.HasComponent<ColliderComponent>(enemyId))
                    continue;

                var enemyPos = world.GetComponent<PositionComponent>(enemyId);
                var enemyCol = world.GetComponent<ColliderComponent>(enemyId);

                for (int i = 0; i < _projectiles.Count; i++)
                {
                    var proj = _projectiles[i];
                    if (CheckCollision(proj.pos, proj.col, enemyPos, enemyCol))
                        ApplyHit(world, proj.entity, proj.pos, proj.col, proj.dmg, proj.vel, proj.hasVel, enemyId);
                }
            }
        }

        private static bool CheckCollision(
            PositionComponent pos1, ColliderComponent col1,
            PositionComponent pos2, ColliderComponent col2)
        {
            float dx = pos1.x - pos2.x;
            float dy = pos1.y - pos2.y;
            float radiusSum = col1.radius + col2.radius;
            return dx * dx + dy * dy <= radiusSum * radiusSum;
        }

        private void ApplyHit(
            World world,
            int projEntity,
            PositionComponent projPos,
            ColliderComponent projCol,
            DamageSourceComponent dmg,
            VelocityComponent projVel,
            bool hasVel,
            int enemyId)
        {
            // 1. 伤害
            if (world.HasComponent<HealthComponent>(enemyId))
            {
                var health = world.GetComponent<HealthComponent>(enemyId);
                health.current -= dmg.damage;
                world.SetComponent(enemyId, health);
            }

            // 2. 击退
            if (dmg.knockBack > 0.001f)
            {
                var enemyPos = world.GetComponent<PositionComponent>(enemyId);
                Vector2 knockBackDir = CalculateKnockBackDirection(projPos, projVel, hasVel, enemyPos);

                if (!world.HasComponent<KnockBackComponent>(enemyId))
                {
                    world.AddComponent(enemyId, new KnockBackComponent(
                        knockBackDir.x * dmg.knockBack,
                        knockBackDir.y * dmg.knockBack,
                        KNOCKBACK_DURATION
                    ));
                }
                else
                {
                    var knockBack = world.GetComponent<KnockBackComponent>(enemyId);
                    knockBack.forceX += knockBackDir.x * dmg.knockBack;
                    knockBack.forceY += knockBackDir.y * dmg.knockBack;
                    knockBack.time = KNOCKBACK_DURATION;
                    world.SetComponent(enemyId, knockBack);
                }
            }

            // 3. 命中动画
            world.AddComponent(enemyId, new AnimationCommandComponent("play", "Hit"));

            // 4. 投射物穿透
            if (world.HasComponent<ProjectileComponent>(projEntity))
            {
                var projectile = world.GetComponent<ProjectileComponent>(projEntity);
                projectile.hit_count++;
                world.SetComponent(projEntity, projectile);

                if (projectile.hit_count >= projectile.pierce)
                    world.DestroyEntity(projEntity);
            }
        }

        private static Vector2 CalculateKnockBackDirection(
            PositionComponent projPos,
            VelocityComponent projVel,
            bool hasVel,
            PositionComponent enemyPos)
        {
            if (hasVel)
            {
                float velMag = Mathf.Sqrt(projVel.x * projVel.x + projVel.y * projVel.y);
                if (velMag > 0.001f)
                    return new Vector2(projVel.x / velMag, projVel.y / velMag);
            }

            float dx = enemyPos.x - projPos.x;
            float dy = enemyPos.y - projPos.y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);

            return dist > 0.001f
                ? new Vector2(dx / dist, dy / dist)
                : Vector2.right;
        }
    }
}