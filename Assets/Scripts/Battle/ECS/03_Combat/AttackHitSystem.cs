using System.Collections.Generic;
using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    public class AttackHitSystem : SystemBase
    {
        private const float KNOCKBACK_DURATION = 0.15f;

        // 邻居缓冲区：预分配，QueryNeighbors 零 GC
        // 128 可覆盖绝大多数密集场景；若出现截断可在此调大
        private readonly int[] _neighborBuffer = new int[256];

        // 复用列表避免热路径分配
        private readonly List<(int entity, PositionComponent pos, ColliderComponent col,
            DamageSourceComponent dmg, VelocityComponent vel, bool hasVel)> _projectiles
            = new List<(int, PositionComponent, ColliderComponent, DamageSourceComponent, VelocityComponent, bool)>();

        public override void Update(World world, float deltaTime)
        {
            // 查询共享的敌人空间索引服务（由 EnemySpatialIndexSystem 每帧构建）
            if (!world.TryGetService<IEnemySpatialIndex>(out var enemyIndex))
                return;

            // ── 1. 收集投射物 ──────────────────────────────────────
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

            if (_projectiles.Count == 0) return;

            // ── 2. 按投射物查询候选敌人，O(P×k) 碰撞检测 ─────────
            for (int i = 0; i < _projectiles.Count; i++)
            {
                var proj = _projectiles[i];
                float queryRadius = proj.col.radius + 1.0f; // 1.0f = 敌人最大碰撞半径上限

                int neighborCount = enemyIndex.QueryEnemies(proj.pos.x, proj.pos.y, queryRadius, _neighborBuffer);

#if UNITY_EDITOR
                if (neighborCount >= _neighborBuffer.Length)
                    Debug.LogWarning(
                        $"[AttackHitSystem] 邻居缓冲区已满（{_neighborBuffer.Length}），可能存在碰撞漏检，请增大 _neighborBuffer 大小。");
#endif

                for (int n = 0; n < neighborCount; n++)
                {
                    int enemyId = _neighborBuffer[n];

                    if (!world.HasComponent<PositionComponent>(enemyId) ||
                        !world.HasComponent<ColliderComponent>(enemyId))
                        continue;

                    var enemyPos = world.GetComponent<PositionComponent>(enemyId);
                    var enemyCol = world.GetComponent<ColliderComponent>(enemyId);

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
            // 0. 命中冷却检查（无敌帧）：冷却中直接跳过
            if (world.HasComponent<HitCooldownComponent>(enemyId))
                return;

            // 命中冷却持续时间：与击退时长对齐，保证手感一致
            world.AddComponent(enemyId, new HitCooldownComponent(KNOCKBACK_DURATION));

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