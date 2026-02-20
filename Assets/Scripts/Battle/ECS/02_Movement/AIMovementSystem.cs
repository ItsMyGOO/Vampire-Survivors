using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// AI 移动系统
    /// 职责: 控制敌人追踪玩家
    /// 优化: 超出激活半径的敌人跳过速度计算，降低远距离大量敌人的 CPU 开销
    /// </summary>
    public class AIMovementSystem : SystemBase
    {
        /// <summary>
        /// 敌人 AI 激活半径。超出此距离的敌人停止追踪计算（速度保持不变）。
        /// 应略大于屏幕可视半径，避免出现 pop-in 现象。
        /// </summary>
        private const float AI_ACTIVE_RADIUS = 30f;
        private const float AI_ACTIVE_RADIUS_SQ = AI_ACTIVE_RADIUS * AI_ACTIVE_RADIUS;

        public override void Update(World world, float deltaTime)
        {
            int playerId = FindPlayer(world);
            if (playerId == -1) return;
            if (!world.HasComponent<PositionComponent>(playerId)) return;

            var playerPos = world.GetComponent<PositionComponent>(playerId);

            world.IterateComponents<EnemyTagComponent>(out int[] ids, out _, out int count);
            for (int i = 0; i < count; i++)
            {
                int entity = ids[i];

                if (!world.HasComponent<PositionComponent>(entity) ||
                    !world.HasComponent<VelocityComponent>(entity))
                    continue;

                var enemyPos = world.GetComponent<PositionComponent>(entity);

                float dx = playerPos.x - enemyPos.x;
                float dy = playerPos.y - enemyPos.y;
                float distSq = dx * dx + dy * dy;

                // 超出激活半径：跳过此帧 AI 计算，节省远距离敌人开销
                if (distSq > AI_ACTIVE_RADIUS_SQ)
                    continue;

                var vel = world.GetComponent<VelocityComponent>(entity);

                float dist = Mathf.Sqrt(distSq);
                if (dist > 0.1f)
                {
                    vel.x = (dx / dist) * vel.speed;
                    vel.y = (dy / dist) * vel.speed;
                }
                else
                {
                    vel.x = 0f;
                    vel.y = 0f;
                }

                world.SetComponent(entity, vel);
            }
        }

        private int FindPlayer(World world)
        {
            foreach (var (entity, _) in world.GetComponents<PlayerTagComponent>())
                return entity;
            return -1;
        }
    }
}